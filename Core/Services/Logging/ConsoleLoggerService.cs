using System;
using System.Threading.Tasks;

namespace DiscordVerifyBot.Core.Services
{
    public class ConsoleLoggerService : ILoggerService
    {
        public void Log(string Message, string Source = null)
        {
            Console.WriteLine($"{DateTime.Now} at {Source} ] {Message}");
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task LogAsync(string Message, string Source = null)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            Log(Message, Source);
        }
    }
}
