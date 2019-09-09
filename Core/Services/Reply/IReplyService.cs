using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace DiscordVerifyBot.Core.Services
{
    public interface IReplyService
    {
        Task ReplyTextAsync(ICommandContext context, string message);

        Task ReplyEmbedAsync(ICommandContext context, string message, string description = null);

        Task ReplyEmbedAsync(ICommandContext context, EmbedBuilder embed);
    }
}
