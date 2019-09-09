using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace DiscordVerifyBot.Core.Services
{
    public class CommandReplyService : IReplyService
    {
        public async Task ReplyEmbedAsync(ICommandContext context, EmbedBuilder embed)
        {
            await context.Channel.SendMessageAsync(embed: embed.Build());
        }

        public async Task ReplyEmbedAsync(ICommandContext context, string message, string description = null)
        {
            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = message,
                    IconUrl = context.Client.CurrentUser.GetAvatarUrl()
                },
                Color = Color.Blue,
                Timestamp = DateTime.UtcNow
            };
            
            if (!string.IsNullOrWhiteSpace(description))
            {
                embed.Description = description;
            }

            await context.Channel.SendMessageAsync(embed: embed.Build());
        }

        public async Task ReplyTextAsync(ICommandContext context, string message)
        {
            await context.Channel.SendMessageAsync(text: message);
        }
    }
}
