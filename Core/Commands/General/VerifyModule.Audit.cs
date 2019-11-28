using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using DiscordVerifyBot.Core.Handlers;

namespace DiscordVerifyBot.Core.Commands.General
{
    public partial class VerifyModule : ModuleBase<SocketCommandContext>
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

            users = users.OrderByDescending(x => Convert.ChangeType(x.PermissionLevel, x.PermissionLevel.GetTypeCode())).ToList();

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

            string permLevelString = "None";
            if(dbUser != null)
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

        #region Verificator Rankings

        [Command("Leaderboard")]
        public async Task ShowVerificatorLeaderboard()
        {
            var forms = VerificationFormDataHandler.GetVerificationFormsByGuild(Context.Guild.Id);

            var groups = forms.GroupBy(x => x.Verifier).OrderByDescending( x => x.Count() );

            var embed = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = $"Showing verification leaderboard",
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl()
                },
                Color = Color.Blue,
                Timestamp = DateTimeOffset.UtcNow
            };

            foreach(var group in groups)
            {
                var user = Context.Guild.GetUser(group.Key);
                var field = new EmbedFieldBuilder()
                {
                    Name = user.Nickname ?? user.Username,
                    Value = group.Count()
                };
                embed.AddField(field);
            }

            await _replyservice.ReplyEmbedAsync(Context, embed);
        }

        #endregion
    }
}
