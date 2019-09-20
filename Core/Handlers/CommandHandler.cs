using System;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using DiscordVerifyBot.Core.Services;

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
    }
}
