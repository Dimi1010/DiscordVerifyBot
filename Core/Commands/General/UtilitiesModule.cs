using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;

using DiscordVerifyBot.Core.Services;

namespace DiscordVerifyBot.Core.Commands.General
{
    class UtilitiesModule : ModuleBase<SocketCommandContext>
    {
        private readonly IReplyService _replyservice;
        private readonly ILoggerService _logger;

        public UtilitiesModule(IReplyService replyService, ILoggerService logger)
        {
            _replyservice = replyService ?? throw new ArgumentNullException(paramName: "replyService");
            _logger = logger ?? throw new ArgumentNullException(paramName: "logger");
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
