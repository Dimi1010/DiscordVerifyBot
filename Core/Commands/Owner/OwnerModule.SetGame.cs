using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace DiscordVerifyBot.Core.Commands.Owner
{
    public partial class OwnerModule
    {
        [Command("SetGame"), Alias("SetG"), Summary("Sets a new Game Activity"),
        RequireOwner]
        public async Task SetGameAsync (string activityType, params string[] gameArgs)
        {
            ActivityType activity;

            switch (activityType)
            {
                case "Playing":
                case "playing":
                    activity = ActivityType.Playing;
                    break;
                case "Listening":
                case "listening":
                    activity = ActivityType.Listening;
                    break;
                case "Watching":
                case "watching":
                    activity = ActivityType.Watching;
                    break;
                case "Streaming":
                case "streaming":
                    activity = ActivityType.Streaming;
                    break;
                default:
                    activity = ActivityType.Playing;
                    break;
            }

            var gameName = string.Join(" ", gameArgs);

            await Context.Client.SetGameAsync(name: gameName, type: activity);
        }

        [Command("ClearGame"), Summary("Clears the Game Activity"),
        RequireOwner]
        public async Task ClearGameAsync()
        {
            await Context.Client.SetGameAsync("");
        }

    }
}
