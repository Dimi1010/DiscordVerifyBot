using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Serilog;
using Serilog.Core;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using DiscordVerifyBot.Core.Services;
using DiscordVerifyBot.Core.Handlers;
using DiscordVerifyBot.Core.Logging;

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

        internal static ManualResetEvent _quitEvent = new ManualResetEvent(false);

        public Program()
        {
            Console.CancelKeyPress += (sender, eArgs) =>
            {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };

            //Creates and/or Updates the database when the program is strated
            using (var DbContext = new SQLiteDatabaseContext())
            {
                DbContext.Database.Migrate();
                //int maxRetries = 3;
                //for(int i = 0; i < maxRetries; i++)
                //{
                //    try
                //    {
                //        DbContext.Database.Migrate();
                //        break;
                //    }
                //    catch (System.NotSupportedException e)
                //    {
                //        DbContext.Database.EnsureDeleted();
                //    }
                //}
            }

            Settings settings;
            using (var DH = new SettingsDataHandler())
            {
                settings = DH.GetSettings();

                #region Logger Creation

                //Gets and converts Log Levels to match between Serilog and integrated Discord.Net Logger
                var LogLevelSerilog = Serilog.Events.LogEventLevel.Information;
                var LogLevelDiscord = LogSeverity.Info;

                if (Enum.IsDefined(typeof(Serilog.Events.LogEventLevel), settings.LogLevel))
                {
                    LogLevelSerilog = (Serilog.Events.LogEventLevel)settings.LogLevel;
                    new LogLevelConverter().SerilogToDiscordNet(LogLevelSerilog, out LogLevelDiscord);
                }

                LoggingLevelSwitch levelSwitch = new LoggingLevelSwitch()
                {
                    MinimumLevel = LogLevelSerilog
                };

                var loggerConfiguration = new LoggerConfiguration()
                    .MinimumLevel.Is(LogLevelSerilog)
                    .Enrich.With(new ThreadIdEnricher())
                    .WriteTo.Console(
                        outputTemplate: "{Timestamp:HH:mm} [{Level}] ({ThreadId}) {Message}{NewLine}{Exception}");
                if (Convert.ToBoolean(settings.RollingLogRetainedFiles))
                {
                    loggerConfiguration.WriteTo.File(
                        path: DH.GetLogFilePath(),
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: settings.RollingLogRetainedFiles,
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message}{NewLine}{Exception}");
                }

                Log.Logger = loggerConfiguration.CreateLogger();
                Log.Debug("Logger Created");

                #endregion

                _client = new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogLevelDiscord
                });

                _commandService = new CommandService(new CommandServiceConfig
                {
                    CaseSensitiveCommands = false,
                    DefaultRunMode = RunMode.Async,
                    LogLevel = LogLevelDiscord
                });
            }

            _serviceProvider = new ServiceProviderFactory(_client, _commandService).Build();

            _commandHandler = new CommandHandler(
                client: _client,
                commandService: _commandService,
                serviceProvider: _serviceProvider,
                replyService: _serviceProvider.GetRequiredService<IReplyService>()
                );

            _client.Log += OnClientLogAsync;
            _client.Ready += OnClientReadyAsync;
        }

        private async Task OnClientReadyAsync()
        {
            await _client.SetGameAsync("your commands.", null, ActivityType.Listening);
        }

        private async Task OnClientLogAsync(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                    Log.Fatal(message.Message);
                    break;
                case LogSeverity.Error:
                    Log.Error(message.Message);
                    break;
                case LogSeverity.Warning:
                    Log.Warning(message.Message);
                    break;
                case LogSeverity.Info:
                    Log.Information(message.Message);
                    break;
                case LogSeverity.Debug:
                    Log.Debug(message.Message);
                    break;
                case LogSeverity.Verbose:
                    Log.Verbose(message.Message);
                    break;
            }
            //await _serviceProvider.GetRequiredService<ILoggerService>().LogAsync(Message: message.Message, Source: message.Source);
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

            Log.Logger.Information(
                "User initiated shutdown.");

            await _client.LogoutAsync();
            await _client.StopAsync();
            await Task.Delay(millisecondsDelay: 1000);
        }
    }
}
