using System;
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
        [Command("List-Last"), Alias("l", "Show-Last")]
        public async Task ListLastForms(int records = 5)
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
    }
}
