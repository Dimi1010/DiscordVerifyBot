using System.Threading.Tasks;

using Serilog;

using Discord.Commands;
using Discord;

namespace DiscordVerifyBot.Core.Commands.General
{
    public partial class VerifyModule : ModuleBase<SocketCommandContext>
    {
        [Command("Execute-Override-Add")]
        [RequireContext(ContextType.Guild)]
        [RequireOwner]
        public async Task AddRolesOverride(IGuildUser user, IRole role, string reason = null)
        {
            await user.AddRoleAsync(role, new RequestOptions() {
                AuditLogReason = $"Role override by {Context.User.Username + Context.User.Discriminator}." + reason == null ? "\n Reason: { reason}" : ""
            });

            Log.Information(
                "User {InvokingUserID} executed role override(ADD) for user {UserID} in guild {GuildID}",
                Context.User.Id, user.Id, Context.Guild.Id
                );

            await _replyservice.ReplyEmbedAsync(context: Context,
                message: $"Override successful.");
        }

        [Command("Execute-Override-Remove")]
        [RequireContext(ContextType.Guild)]
        [RequireOwner]
        public async Task RemoveRolesOverride(IGuildUser user, IRole role, string reason = null)
        {
            await user.RemoveRoleAsync(role, new RequestOptions()
            {
                AuditLogReason = $"Role override by {Context.User.Username + Context.User.Discriminator}." + reason == null ? "\n Reason: {reason}" : ""
            });

            Log.Information(
                "User {InvokingUserID} executed role override(REMOVE) for user {UserID} in guild {GuildID}",
                Context.User.Id, user.Id, Context.Guild.Id
                );

            await _replyservice.ReplyEmbedAsync(context: Context,
                message: $"Override successful.");
        }
    }
}
