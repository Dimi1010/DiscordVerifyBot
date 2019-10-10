﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using DiscordVerifyBot.Core.Services;
using DiscordVerifyBot.Core.Utility;
using System.Linq;

namespace DiscordVerifyBot.Core.Handlers
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly ILoggerService _loggerService;
        private readonly IReplyService _replyService;
        private readonly IServiceProvider _serviceProvider;

        private readonly string _prefix;

        public CommandHandler(
            DiscordSocketClient client,
            CommandService commandService, 
            IServiceProvider serviceProvider,
            ILoggerService loggerService,
            IReplyService replyService)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _loggerService = loggerService ?? throw new ArgumentNullException(nameof(loggerService));
            _replyService = replyService ?? throw new ArgumentNullException(nameof(replyService));

            using ( var DH = new SettingsDataHandler())
            {
                _prefix = DH.GetSettings().DefaultPrefix;
            }
        }

        public async Task SetupAsync()
        {
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

            _commandService.CommandExecuted += OnCommandExecutedAsync;
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage MessageParam)
        {
            var Message = MessageParam as SocketUserMessage;
            var Context = new SocketCommandContext(_client, Message);

            if (Context.Message == null || Context.Message.Content == "") return; //Cancels command recognition if there is no message content
            if (Context.User.IsBot) return; //Cancels command recognition if sender is a bot

            int prefixPos = 0;

            //Cancels command recognition if there is no prefix
            if (!(Message.HasStringPrefix(_prefix, ref prefixPos) || Message.HasMentionPrefix(_client.CurrentUser, ref prefixPos))) return;
            _ = await _commandService.ExecuteAsync(Context, prefixPos, _serviceProvider);
        }

        private async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!result.IsSuccess)
            {
                if (!string.IsNullOrEmpty(result.ErrorReason))
                {
                    _loggerService.Log(
                        Message: $"Error Processing Command. Text: {context.Message.Content} | Error: {result.ErrorReason}",
                        Source: "Commands"
                        );
                    // TODO: Check if CommandInfo is present if command fails
                    if (result.Error == CommandError.UnknownCommand)
                    {
                        var similar = await MostSimilarCommandAsync(context.Message.Content);

                        await _replyService.ReplyEmbedAsync(context, message: "Command Failed", description: result.ErrorReason + $"\nDid you mean '--{similar.Item1}' instead? ");
                    }
                    else
                        await _replyService.ReplyEmbedAsync(context, message: "Command Failed", description: result.ErrorReason);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(result.ErrorReason))
                {
                    _loggerService.Log(
                        Message: $"Processing Command. Text: {context.Message.Content} | Result: {result.ErrorReason}",
                        Source: "Commands"
                        );
                }
            }
        }

        private async Task<Tuple<string, CommandInfo>> MostSimilarCommandAsync(string inputText, float threshhold = 0.7f)
        {
            if (string.IsNullOrWhiteSpace(inputText)) return null;
            if (threshhold < 0) threshhold = 0;
            if (threshhold > 1) threshhold = 1;

            var commandScores = new List<Tuple<string, double, CommandInfo>>();

            foreach (var command in _commandService.Commands)
            {
                double distance = LevenshteinDistance.Compute(command.Name, inputText);

                commandScores.Add(Tuple.Create(command.Name, distance, command));
                
                foreach(var alias in command.Aliases)
                {
                    if (alias == command.Name) continue;

                    double alias_distance = LevenshteinDistance.Compute(alias, inputText);

                    commandScores.Add(Tuple.Create(alias, distance, command));
                }
            }

            var scores = commandScores.Select(x => x.Item2);

            var probabilities_inverted = Softmax.Compute(scores);
            var probabilities = probabilities_inverted.Select(x => 1 - x).ToList();

            var commandProbabilities = new List<Tuple<string, double, CommandInfo>>();

            if (commandScores.Count != probabilities.Count())
                throw new ArgumentException(nameof(commandScores) + " length is not equal to " + nameof(probabilities));

            for(int i = 0; i < commandScores.Count; ++i)
            {
                commandProbabilities.Add(Tuple.Create(commandScores[i].Item1, probabilities[i], commandScores[i].Item3));
            }

            commandProbabilities = commandProbabilities.Where(x => x.Item2 >= threshhold).OrderByDescending(x => x.Item2).ToList();
            
            if(commandProbabilities.Count > 0)
            {
                var top = commandProbabilities.FirstOrDefault();
                return Tuple.Create(top.Item1, top.Item3);
            }
            else
                return null;
        }
    }
}
