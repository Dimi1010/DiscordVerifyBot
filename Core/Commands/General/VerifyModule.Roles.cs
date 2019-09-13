using System.Threading.Tasks;
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

            _logger.Log(Message: $"User {Context.User.Id} in {Context.Guild.Id} added role {role.Id} with action { action.ToString() }", Source:"Commands");

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

            _logger.Log(Message: $"User {Context.User.Id} in {Context.Guild.Id} removed role {role.Id} from the database", Source: "Commands");

            await _replyservice.ReplyEmbedAsync(context: Context,
                message: $"Role {role.Name} has been removed from the database.");
        }
    }
}
