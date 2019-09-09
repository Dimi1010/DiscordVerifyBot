using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace DiscordVerifyBot.Core.Commands.Owner
{
    public partial class OwnerModule
    {   
        [Command("SetStatus"), Alias("SetS"), Summary("Sets the bot status"),
        RequireOwner]
        public async Task SetStatusAsync(string status)
        {
            UserStatus userStatus;
            switch (status)
            {
                case "Online":
                case "online":
                    userStatus = UserStatus.Online;
                    break;
                case "Idle":
                case "idle":
                    userStatus = UserStatus.Idle;
                    break;
                case "DnD":
                case "dnd":
                    userStatus = UserStatus.DoNotDisturb;
                    break;
                case "Invisible":
                case "invisible":
                    userStatus = UserStatus.Invisible;
                    break;
                default:
                    userStatus = UserStatus.Online;
                    break;
            }
            await Context.Client.SetStatusAsync(userStatus);
        }
    }
}
