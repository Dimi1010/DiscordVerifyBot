using System;
using Microsoft.Extensions.DependencyInjection;

using Discord.Commands;
using Discord.WebSocket;

namespace DiscordVerifyBot.Core.Services
{
    public class ServiceProviderFactory
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public ServiceProviderFactory(DiscordSocketClient client = null, CommandService commands = null)
        {
            _client = client ?? new DiscordSocketClient();
            _commands = commands ?? new CommandService();
        }

        public IServiceProvider Build()
            => new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commands)
            .AddSingleton<ILoggerService, ConsoleLoggerService>()
            .AddSingleton<IReplyService, CommandReplyService>()
            .BuildServiceProvider();
    }
}