using System;
using System.Threading.Tasks;
using Discord.Commands;

using DiscordVerifyBot.Core.Services;

namespace DiscordVerifyBot.Core.Commands.General
{
    class UtilitiesModule : ModuleBase<SocketCommandContext>
    {
        private readonly IReplyService _replyservice;

        public UtilitiesModule(IReplyService replyService)
        {
            _replyservice = replyService ?? throw new ArgumentNullException(paramName: "replyService");
        }

        #region Utility

        [Command("Ping"), Summary("Responds with bot latency")]
        public async Task PingAsync()
        {
            await _replyservice.ReplyEmbedAsync(context: Context, message: "Pong", description: "Latency to Gateway: " + Context.Client.Latency);
        }

        #endregion
    }
}
