using System.Threading.Tasks;

using Serilog;

using Discord;
using Discord.Commands;
using DiscordVerifyBot.Core.Handlers;
using DiscordVerifyBot.Resources.Database.Model;

namespace DiscordVerifyBot.Core.Commands.General
{
    public partial class VerifyModule : ModuleBase<SocketCommandContext>
    {
        [Command("Add-Role")]
        [RequireContext(ContextType.Guild)]
        [RequireOwner(Group = "Invoke Access")]
        [RequireUserPermission(GuildPermission.ManageRoles, Group = "Invoke Access")]
        public async Task AddRole(string condition, IRole role)
        {
            condition = condition.ToLower();

            DiscordRole.ActionType action;
            switch (condition)
            {
                case "add":
                    action = DiscordRole.ActionType.Add;
                    break;
                case "remove":
                    action = DiscordRole.ActionType.Remove;
                    break;
                default:
                    await _replyservice.ReplyEmbedAsync(context: Context,
                        message: "Command did not specify action type.",
                        description: "Please use the syntax: 'Add-Role [Add or Remove] <RoleMention>'");
                    return;
            }

            await DiscordRoleDataHandler.AddGuildRole(
                roleId: role.Id,
                guildId: Context.Guild.Id,
                action: action
                );

            Log.Information(
                "User {UserId} in {GuildId} added role {RoleId} with action {Action}",
                Context.User.Id, Context.Guild.Id, role.Id, action.ToString()
                );

            await _replyservice.ReplyEmbedAsync(context: Context,
                message: $"Role {role.Name} has been added with the action {action.ToString()}.");
        }

        [Command("Remove-Role")]
        [RequireContext(ContextType.Guild)]
        [RequireOwner(Group = "Invoke Access")]
        [RequireUserPermission(GuildPermission.ManageRoles, Group = "Invoke Access")]
        public async Task RemoveRole(IRole role)
        {
            await DiscordRoleDataHandler.RemoveGuildRole(role.Id, Context.Guild.Id);

            Log.Information(
                "User {UserID} in {GuildID} removed role {RoleID} from the database",
                Context.User.Id, Context.Guild.Id, role.Id
                );

            await _replyservice.ReplyEmbedAsync(context: Context,
                message: $"Role {role.Name} has been removed from the database.");
        }
    }
}
