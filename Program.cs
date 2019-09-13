using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using DiscordVerifyBot.Core.Services;

using DiscordVerifyBot.Core.Handlers;

using DiscordVerifyBot.Resources;
using DiscordVerifyBot.Resources.Database;

namespace DiscordVerifyBot
{
    class Program
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;
        private readonly CommandHandler _commandHandler;

        private static ManualResetEvent _quitEvent = new ManualResetEvent(false);

        public Program()
        {
            Console.CancelKeyPress += (sender, eArgs) =>
            {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };

            Settings settings;
            using (var DH = new SettingsDataHandler())
            {
                settings = DH.GetSettings();
            }

            //Creates and/or Updates the database when the program is strated
            using (var DbContext = new SQLiteDatabaseContext())
            {
                DbContext.Database.Migrate();
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

            using(var DH = new SettingsDataHandler())
            {
                Bot_Token = DH.GetSettings().BotToken;
            }

            //Awaits Bot Login / Start Confirmation
            await _client.LoginAsync(TokenType.Bot, Bot_Token);
            await _client.StartAsync();

            _quitEvent.WaitOne();
        }
    }
}
