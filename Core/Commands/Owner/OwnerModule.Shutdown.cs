using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordVerifyBot.Core.Commands.Owner
{
    public partial class OwnerModule
    {
        [Command("Shutdown"), Alias("Exit"), Summary("Shuts down the bot"),
        RequireOwner]
        public async Task ShutdownAsync()
        {
            await Context.Client.LogoutAsync();
            await Context.Client.StopAsync();
            await Task.Delay(millisecondsDelay: 1000);

            Program._quitEvent.Set();
        }
    }
}
