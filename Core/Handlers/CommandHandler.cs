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
        private readonly CommandService _commands;
        private readonly ILoggerService _loggerService;
        private readonly IServiceProvider _service;

        private readonly string _prefix;

        public CommandHandler(IServiceProvider service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _client = service.GetRequiredService<DiscordSocketClient>();
            _commands = service.GetRequiredService<CommandService>();
            _loggerService = service.GetRequiredService<ILoggerService>();

            using ( var DH = new SettingsDH())
            {
                _prefix = DH.GetSettings().DefaultPrefix;
            }
        }

        public async Task SetupAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _service);

            _commands.CommandExecuted += OnCommandExecutedAsync;
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
            _ = await _commands.ExecuteAsync(Context, prefixPos, _service);
        }

        private async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!result.IsSuccess)
            {
                if (!string.IsNullOrEmpty(result.ErrorReason))
                {
                    _loggerService.Log(
                        Message: $"Error Handling Command. Text: {context.Message.Content} | Error: {result.ErrorReason}",
                        Source: "Commands"
                        );

                    //TODO: Separate Replies into their own service
                    var embedError = new EmbedBuilder()
                    {
                        Author = new EmbedAuthorBuilder()
                        {
                            Name = "Command Failed",
                            IconUrl = context.Client.CurrentUser.GetAvatarUrl()
                        },
                        Description = result.ErrorReason,
                        Color = Color.Blue,
                        Timestamp = DateTime.UtcNow
                    };

                    await context.Channel.SendMessageAsync(embed: embedError.Build());
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(result.ErrorReason))
                {
                    Console.WriteLine($"{DateTime.Now} at Commands ] Handling Command. Text: {context.Message.Content} | Result: {result.ErrorReason}");
                    _loggerService.Log(
                        Message: $"Handling Command. Text: {context.Message.Content} | Result: {result.ErrorReason}",
                        Source: "Commands"
                        );

                    //TODO: Separate Replies into their own service
                    var embedError = new EmbedBuilder()
                    {
                        Author = new EmbedAuthorBuilder()
                        {
                            Name = "Command Successful",
                            IconUrl = context.Client.CurrentUser.GetAvatarUrl()
                        },
                        Description = result.ErrorReason,
                        Color = Color.Blue,
                        Timestamp = DateTime.UtcNow
                    };

                    await context.Channel.SendMessageAsync(embed: embedError.Build());
                }
            }
        }
    }
}
