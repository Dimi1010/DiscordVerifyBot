using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord.Commands;

using DiscordVerifyBot.Core.Handlers;
using DiscordVerifyBot.Resources.Database.Model;
using System.Linq;
using Discord;

namespace DiscordVerifyBot.Core.Commands.General
{
    public partial class VerifyModule : ModuleBase<SocketCommandContext>
    {
        [Command("Execite-Override-Add")]
        [RequireContext(ContextType.Guild)]
        [RequireOwner]
        public async Task AddRolesOverride(IGuildUser user)
        {
            var roleQuery = DiscordRoleDataHandler.GetGuildRoles(Context.Guild.Id);

            var toBeAddedQuery = roleQuery.Where(x => x.Action == DiscordRole.ActionType.Add);
            
            List<IRole> toBeAdded = new List<IRole>();

            foreach (var role in toBeAddedQuery)
            {
                var socketRole = Context.Guild.GetRole(role.RoleId);

                toBeAdded.Add(socketRole);
            }

            await user.AddRolesAsync(toBeAdded);

            _logger.Log(Message: $"User {Context.User.Id} executed role override(ADD) for user {user.Id} in guild {Context.Guild.Id}", Source: "Commands");

            await _replyservice.ReplyEmbedAsync(context: Context,
                message: $"Override successful.");
        }

        [Command("Execute-Override-Remove")]
        [RequireContext(ContextType.Guild)]
        [RequireOwner]
        public async Task RemoveRolesOverride(IGuildUser user)
        {
            var roleQuery = DiscordRoleDataHandler.GetGuildRoles(Context.Guild.Id);

            var toBeRemovedQuery = roleQuery.Where(x => x.Action == DiscordRole.ActionType.Remove);

            List<IRole> toBeRemoved = new List<IRole>();

            foreach (var role in toBeRemovedQuery)
            {
                var socketRole = Context.Guild.GetRole(role.RoleId);

                toBeRemoved.Add(socketRole);
            }

            await user.RemoveRolesAsync(toBeRemoved);

            _logger.Log(Message: $"User {Context.User.Id} executed role override(REMOVE) for user {user.Id} in guild {Context.Guild.Id}", Source: "Commands");

            await _replyservice.ReplyEmbedAsync(context: Context,
                message: $"Override successful.");
        }
    }
}
