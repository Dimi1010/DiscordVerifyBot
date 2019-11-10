﻿using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.Commands;

using DiscordVerifyBot.Core.Handlers;

namespace DiscordVerifyBot.Core.Commands.General
{
    partial class VerifyModule : ModuleBase<SocketCommandContext>
    {
        #region Verification Forms

        [Command("Show-Last"), Alias("sl")]
        public async Task ShowLastForms(int records = 5)
        {
            if(records <= 0)
            {
                await _replyservice.ReplyEmbedAsync(context: Context, message: "The number of selected records must be positive.");
                return;
            }

            var forms = VerificationFormDataHandler.GetVerificationFormsByGuild(Context.Guild.Id, records);

            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = $"Showing last {forms.Count} verification forms",
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl()
                },
                Color = Color.Blue,
                Timestamp = DateTimeOffset.UtcNow
            };

            foreach(var form in forms)
            {
                var field = new EmbedFieldBuilder
                {
                    Name = Context.Guild.GetUser(form.Verified).Username,
                    Value = $"Verified by {form.Verifier} at {form.IssuedUtc} UTC.\n" + (form.IsApproved.HasValue ? (form.IsApproved.Value ? "Approved" : "Denied") + $"by { form.Approver } at {form.ApprovedUtc} UTC." : "Pending approval...")
                };

                embed.AddField(field);
            }

            await _replyservice.ReplyEmbedAsync(Context, embed);
        }

        #endregion

        #region User Permissions

        [Command("Show-Permissions")]
        public async Task ShowUserPermissions()
        {
            var users = DiscordUserDataHandler.GetGuildUsers(Context.Guild.Id);

            if (users.Count < 1)
            {
                await _replyservice.ReplyEmbedAsync(context: Context, message: "No users have special permissions for this bot.");
                return;
            }

            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = $"Showing user permissions.",
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl()
                },
                Color = Color.Blue,
                Timestamp = DateTimeOffset.UtcNow
            };

            foreach(var user in users)
            {
                string permLevelString;
                switch (user.PermissionLevel)
                {
                    case Resources.Database.Model.DiscordGuildUser.PermissionLevels.Verify:
                        permLevelString = "Verifier";
                        break;
                    case Resources.Database.Model.DiscordGuildUser.PermissionLevels.Approve:
                        permLevelString = "Approver";
                        break;
                    default:
                        permLevelString = "None";
                        break;
                }

                var field = new EmbedFieldBuilder
                {
                    Name = Context.Guild.GetUser(user.UserId).Username,
                    Value = "Permission: " + permLevelString
                };

                embed.AddField(field);
            }

            await _replyservice.ReplyEmbedAsync(Context, embed);
        }

        [Command("Show-Permissions")]
        public async Task ShowUserPermissions(IGuildUser user)
        {
            var dbUser = DiscordUserDataHandler.GetGuildUserById(user.Id, Context.Guild.Id);

            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = $"Showing user permissions.",
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl()
                },
                Color = Color.Blue,
                Timestamp = DateTimeOffset.UtcNow
            };

            string permLevelString;
            switch (dbUser.PermissionLevel)
            {
                case Resources.Database.Model.DiscordGuildUser.PermissionLevels.Verify:
                    permLevelString = "Verifier";
                    break;
                case Resources.Database.Model.DiscordGuildUser.PermissionLevels.Approve:
                    permLevelString = "Approver";
                    break;
                default:
                    permLevelString = "None";
                    break;
            }

            var field = new EmbedFieldBuilder
            {
                Name = user.Username,
                Value = "Permission: " + permLevelString
            };

            embed.AddField(field);
            
            await _replyservice.ReplyEmbedAsync(Context, embed);
        }

        #endregion

    }
}
