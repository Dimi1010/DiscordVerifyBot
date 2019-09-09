using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace DiscordVerifyBot.Core.Commands.Owner
{
    public partial class OwnerModule
    {
        [Command("Leave"), Summary("Makes the bot leave specific guild"),
        RequireOwner]
        public async Task LeaveGuildAsync(ulong guildId)
        {
            var client = Context.Client as IDiscordClient;
            var guild = await client.GetGuildAsync(guildId);

            if ( guild == null)
            {
                await Context.Channel.SendMessageAsync($"Bot is not a member of the guild with id {guildId}");
                return;
            }

            await guild.LeaveAsync();
            await Context.Channel.SendMessageAsync($"Bot left guild {guild.Name} successfully");
        }
    }
}
