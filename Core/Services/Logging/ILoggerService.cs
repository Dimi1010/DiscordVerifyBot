using System.Threading.Tasks;

namespace DiscordVerifyBot.Core.Services
{
    public interface ILoggerService
    {
        void Log(string Message, string Source = null);

        Task LogAsync(string Message, string Source = null);
    }
}
