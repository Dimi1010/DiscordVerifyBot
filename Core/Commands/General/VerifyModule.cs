using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord;
using Discord.Commands;

using DiscordVerifyBot.Core.Services;
using DiscordVerifyBot.Core.Handlers;
using DiscordVerifyBot.Resources.Database.Model;

namespace DiscordVerifyBot.Core.Commands.General
{
    public partial class VerifyModule : ModuleBase<SocketCommandContext>
    {
        private readonly IReplyService _replyservice;
        private readonly ILoggerService _logger;

        public VerifyModule(IReplyService replyService, ILoggerService logger)
        {
            _replyservice = replyService ?? throw new ArgumentNullException(paramName: "replyService");
            _logger = logger ?? throw new ArgumentNullException(paramName: "logger");
        }

        #region L1User

        [Command("Add-Verifier"), Alias("add-v"), Summary("Adds a user to the guild's verifier list")]
        [RequireContext(ContextType.Guild)]
        [RequireOwner(Group = "Invoke Access")]
        [RequireUserPermission(GuildPermission.ManageRoles, Group = "Invoke Access")]
        public async Task AddL1User(IGuildUser user)
        {
            await DiscordUserDataHandler.AddGuildUser(user.Id, user.GuildId, DiscordGuildUser.PermissionLevels.Verify);

            _logger.Log(
                Message: $"Adding {user.Id} to the L1 list of guild {Context.Guild.Id}",
                Source: "Commands");

            await _replyservice.ReplyEmbedAsync(context: Context,
                message: $"User {user.Username} has been granted verifier permissions.");
        }

        [Command("Remove-Verifier"), Alias("remove-v"), Summary("Removes a user to the guild's verifier list")]
        [RequireContext(ContextType.Guild)]
        [RequireOwner(Group = "Invoke Access")]
        [RequireUserPermission(GuildPermission.ManageRoles, Group = "Invoke Access")]
        public async Task RemoveL1User(IGuildUser user)
        {
            var res = DiscordUserDataHandler.GetGuildUserById(user.Id, Context.Guild.Id);
            if(res == null || res.PermissionLevel != DiscordGuildUser.PermissionLevels.Verify)
            {
                await _replyservice.ReplyEmbedAsync(context: Context,
                    message: $"User {user.Username} does not have verifier permissions.");
            }
            else
            {
                await DiscordUserDataHandler.RemoveGuildUser(res);

                _logger.Log(
                Message: $"Removing {user.Id} from the L1 list of guild {Context.Guild.Id}",
                Source: "Commands");

                await _replyservice.ReplyEmbedAsync(context: Context,
                    message: $"User {user.Username}'s verifier permissions have been revoked.");
            }
        }

        #endregion

        #region L2User

        [Command("Add-Approved"), Alias("add-a"), Summary("Adds a user to the guild's approver list")]
        [RequireContext(ContextType.Guild)]
        [RequireOwner(Group = "Invoke Access")]
        [RequireUserPermission(GuildPermission.ManageRoles, Group = "Invoke Access")]
        public async Task AddL2User(IGuildUser user)
        {
            await DiscordUserDataHandler.AddGuildUser(user.Id, user.GuildId, DiscordGuildUser.PermissionLevels.Approve);

            _logger.Log(
                Message: $"Adding {user.Id} to the L2 list of guild {Context.Guild.Id}",
                Source: "Commands");

            await _replyservice.ReplyEmbedAsync(context: Context,
                message: $"User {user.Username} has been granted approver permissions.");
        }

        [Command("Remove-Approver"), Alias("remove-a"), Summary("Removes a user to the guild's approver list")]
        [RequireContext(ContextType.Guild)]
        [RequireOwner(Group = "Invoke Access")]
        [RequireUserPermission(GuildPermission.ManageRoles, Group = "Invoke Access")]
        public async Task RemoveL2User(IGuildUser user)
        {
            var res = DiscordUserDataHandler.GetGuildUserById(user.Id, Context.Guild.Id);
            if (res == null || res.PermissionLevel != DiscordGuildUser.PermissionLevels.Approve)
            {
                await _replyservice.ReplyEmbedAsync(context: Context,
                    message: $"User {user.Username} does not have approver permissions.");
            }
            else
            {
                await DiscordUserDataHandler.RemoveGuildUser(res);

                _logger.Log(
                Message: $"Removing {user.Id} from the L1 list of guild {Context.Guild.Id}",
                Source: "Commands");

                await _replyservice.ReplyEmbedAsync(context: Context,
                    message: $"User {user.Username}'s approver permissions have been revoked.");
            }
        }

        #endregion

        #region Verification

        [Command("Verify")]
        [RequireContext(ContextType.Guild)]
        public async Task Verify(IGuildUser user)
        {
            var invokingUser = DiscordUserDataHandler.GetGuildUserById(Context.User.Id, Context.Guild.Id);
            if ( invokingUser == null)
            {
                await _replyservice.ReplyEmbedAsync(context: Context,
                    message: "You do not have permissions to invoke this command.");
                return;
            }

            var form = new VerificationForm
            {
                GuildId = Context.Guild.Id,
                Verified = user.Id,
                Verifier = Context.User.Id,
                IssuedUtc = DateTime.UtcNow
            };

            if (invokingUser.PermissionLevel == DiscordGuildUser.PermissionLevels.Approve)
            {
                form.Approver = Context.User.Id;
                form.IsApproved = true;

                var query = DiscordRoleDataHandler.GetGuildRoles(Context.Guild.Id);

                var toBeAddedQuery = query.Where(x => x.Action == DiscordRole.ActionType.Add);
                var toBeRemovedQuery = query.Where(x => x.Action == DiscordRole.ActionType.Remove);

                List<IRole> toBeAdded = new List<IRole>();
                List<IRole> toBeRemoved = new List<IRole>();

                foreach (var role in toBeAddedQuery)
                {
                    var socketRole = Context.Guild.GetRole(role.RoleId);

                    toBeAdded.Add(socketRole);
                }

                foreach (var role in toBeRemovedQuery)
                {
                    var socketRole = Context.Guild.GetRole(role.RoleId);

                    toBeRemoved.Add(socketRole);
                }

                await user.AddRolesAsync(toBeAdded);
                await user.RemoveRolesAsync(toBeRemoved);

                await VerificationFormDataHandler.AddFullVerificationForm(form);

                var message = $"User {user.Username} has been verified ";
                if(form.Verifier != Context.User.Id)
                    message += $"by { Context.Guild.GetUser(form.Verifier).Username } ";
                message += "and approved.";

                await _replyservice.ReplyEmbedAsync(context: Context,
                    message: message);

                _logger.Log(Message: $"User {user.Id} verified and approved by {Context.User.Id} in guild {Context.Guild.Id}", Source: "Commands");
            }
            else {
                try
                {
                    await VerificationFormDataHandler.AddPendingVerificationForm(form);
                }
                catch (VerificationFormExistsException)
                {
                    await _replyservice.ReplyEmbedAsync(context: Context,
                        message: $"User {user.Username} already has a pending verification form");
                }
            }
        }

        #endregion

        #region Pending and Approval
        [Command("Pending")]
        [RequireContext(ContextType.Guild)]
        public async Task GetPendingForGuild()
        {
            var query = VerificationFormDataHandler.GetPendingVerificationFormsByGuild(Context.Guild.Id);

            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = "Pending Verifications",
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl()
                },
                Color = Color.Blue,
                Timestamp = DateTime.UtcNow
            };



            foreach(var form in query)
            {
                var field = new EmbedFieldBuilder
                {
                    Name = Context.Guild.GetUser(form.Verified).Username,
                    Value = $"Verifier: { Context.Guild.GetUser(form.Verifier).Username } \n Issued: { form.IssuedUtc.ToString(format: "dd/MM/yyyy HH:mm") }"
                };

                embed.AddField(field);
            }

            await _replyservice.ReplyEmbedAsync(context: Context, embed: embed);
        }

        [Command("Approve")]
        [RequireContext(ContextType.Guild)]
        public async Task ApproveUser(IGuildUser user)
        {
            var invokingUser = DiscordUserDataHandler.GetGuildUserById(Context.User.Id, Context.Guild.Id);
            if (invokingUser == null || invokingUser.PermissionLevel != DiscordGuildUser.PermissionLevels.Approve)
            {
                await _replyservice.ReplyEmbedAsync(context: Context,
                    message: "You do not have permissions to invoke this command.");
                return;
            }

            var query = VerificationFormDataHandler.GetPendingVerificationFormsByGuild(Context.Guild.Id).Where(x => x.Verified == user.Id);

            if(query.Count() < 1)
            {
                await _replyservice.ReplyEmbedAsync(context: Context,
                    message: $"No pending verification for user { user.Username }");

                return;
            }

            var roleQuery = DiscordRoleDataHandler.GetGuildRoles(Context.Guild.Id);

            var toBeAddedQuery = roleQuery.Where(x => x.Action == DiscordRole.ActionType.Add);
            var toBeRemovedQuery = roleQuery.Where(x => x.Action == DiscordRole.ActionType.Remove);

            List<IRole> toBeAdded = new List<IRole>();
            List<IRole> toBeRemoved = new List<IRole>();

            foreach (var role in toBeAddedQuery)
            {
                var socketRole = Context.Guild.GetRole(role.RoleId);

                toBeAdded.Add(socketRole);
            }

            foreach (var role in toBeRemovedQuery)
            {
                var socketRole = Context.Guild.GetRole(role.RoleId);

                toBeRemoved.Add(socketRole);
            }

            await user.AddRolesAsync(toBeAdded);
            await user.RemoveRolesAsync(toBeRemoved);

            var form = query.FirstOrDefault();
            form.Approver = Context.User.Id;
            form.IsApproved = true;
            await VerificationFormDataHandler.AddFullVerificationForm(form);

            await _replyservice.ReplyEmbedAsync(context: Context,
                message: $"User {user.Username} has been approved.");

            _logger.Log(Message: $"User {user.Id} approved by {Context.User.Id} in guild {Context.Guild.Id}", Source: "Commands");
        }

        [Command("Approve-All"), Alias("Approve-a")]
        [RequireContext(ContextType.Guild)]
        public async Task ApproveAll()
        {
            var invokingUser = DiscordUserDataHandler.GetGuildUserById(Context.User.Id, Context.Guild.Id);
            if (invokingUser == null || invokingUser.PermissionLevel != DiscordGuildUser.PermissionLevels.Approve)
            {
                await _replyservice.ReplyEmbedAsync(context: Context,
                    message: "You do not have permissions to invoke this command.");
                return;
            }

            var query = VerificationFormDataHandler.GetPendingVerificationFormsByGuild(Context.Guild.Id);

            if (query.Count() < 1)
            {
                await _replyservice.ReplyEmbedAsync(context: Context,
                    message: $"No pending verifications");

                return;
            }

            var roleQuery = DiscordRoleDataHandler.GetGuildRoles(Context.Guild.Id);

            var toBeAddedQuery = roleQuery.Where(x => x.Action == DiscordRole.ActionType.Add);
            var toBeRemovedQuery = roleQuery.Where(x => x.Action == DiscordRole.ActionType.Remove);

            List<IRole> toBeAdded = new List<IRole>();
            List<IRole> toBeRemoved = new List<IRole>();

            foreach (var role in toBeAddedQuery)
            {
                var socketRole = Context.Guild.GetRole(role.RoleId);

                toBeAdded.Add(socketRole);
            }

            foreach (var role in toBeRemovedQuery)
            {
                var socketRole = Context.Guild.GetRole(role.RoleId);

                toBeRemoved.Add(socketRole);
            }

            foreach(var form in query)
            {

                var user = Context.Guild.GetUser(form.Verified);

                await user.AddRolesAsync(toBeAdded);
                await user.RemoveRolesAsync(toBeRemoved);

                form.Approver = Context.User.Id;
                form.IsApproved = true;
                await VerificationFormDataHandler.AddFullVerificationForm(form);
            }

            await _replyservice.ReplyEmbedAsync(context: Context,
                message: $"The pending forms ({ query.Count() }) have been bulk approved by { Context.User.Username }");

            _logger.Log(Message: $"Bulk approval ({ query.Count() } forms) by {Context.User.Id} in guild {Context.Guild.Id}", Source: "Commands");
        }

        [Command("Deny")]
        [RequireContext(ContextType.Guild)]
        public async Task DenyUser(IGuildUser user)
        {
            var invokingUser = DiscordUserDataHandler.GetGuildUserById(Context.User.Id, Context.Guild.Id);
            if (invokingUser == null || invokingUser.PermissionLevel != DiscordGuildUser.PermissionLevels.Approve)
            {
                await _replyservice.ReplyEmbedAsync(context: Context,
                    message: "You do not have permissions to invoke this command.");
                return;
            }

            var query = VerificationFormDataHandler.GetPendingVerificationFormsByGuild(Context.Guild.Id).Where(x => x.Verified == user.Id);

            if (query.Count() < 1)
            {
                await _replyservice.ReplyEmbedAsync(context: Context,
                    message: $"No pending verification for user { user.Username }");

                return;
            }

            var form = query.FirstOrDefault();
            form.Approver = Context.User.Id;
            form.IsApproved = false;
            await VerificationFormDataHandler.AddFullVerificationForm(form);

            await _replyservice.ReplyEmbedAsync(context: Context,
                message: $"User {user.Username} has been denied verification.");

            _logger.Log(Message: $"User {user.Id} denied by {Context.User.Id} in guild {Context.Guild.Id}", Source: "Commands");
        }

        #endregion
    }
}
