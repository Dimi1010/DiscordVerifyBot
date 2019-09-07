using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using DiscordVerifyBot.Core.Services;

using DiscordVerifyBot.Core.Handlers;

using DiscordVerifyBot.Resources;

namespace DiscordVerifyBot
{
    class Program
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;
        private readonly CommandHandler _commandHandler;

        public Program()
        {
            Settings settings;
            using (var DH = new SettingsDH())
            {
                settings = DH.GetSettings();
            }

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = Enum.IsDefined(typeof(LogSeverity), settings.LogLevel) ? (LogSeverity)settings.LogLevel : LogSeverity.Info
            });

            _commandService = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                LogLevel = Enum.IsDefined(typeof(LogSeverity), settings.LogLevel) ? (LogSeverity)settings.LogLevel : LogSeverity.Info
            });

            _serviceProvider = new ServiceProviderFactory(_client, _commandService).Build();

            _commandHandler = new CommandHandler(_serviceProvider);

            _client.Log += OnClientLogAsync;
            _client.Ready += OnClientReadyAsync;
        }

        private async Task OnClientReadyAsync()
        {
            await _client.SetGameAsync("to your commands.", null, ActivityType.Listening);
        }

        private async Task OnClientLogAsync(LogMessage message)
        {
            await _serviceProvider.GetRequiredService<ILoggerService>().LogAsync(Message: message.Message, Source: message.Source);
        }

        public static void Main()
            => new Program().MainAsync().GetAwaiter().GetResult();
        public async Task MainAsync()
        {
            await _commandHandler.SetupAsync();

            //Fetches the Bot Token From File
            string Bot_Token = "";

            using(var DH = new SettingsDH())
            {
                await DH.SaveStockSettings();
                Bot_Token = DH.GetSettings().BotToken;
            }

            Environment.Exit(0);

            //Awaits Bot Login / Start Confirmation
            await _client.LoginAsync(TokenType.Bot, Bot_Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
