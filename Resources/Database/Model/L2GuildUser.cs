using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace DiscordVerifyBot.Resources.Database.Model
{
    public class L2GuildUser
    {
        /// <summary>
        /// ID of the data entry
        /// </summary>
        [Key] public ulong Id { get; set; }

        /// <summary>
        /// Snowflake ID of the discord user
        /// </summary>
        [Required] public ulong UserId { get; set; }

        /// <summary>
        /// Snowflake Id of the assosiated guild
        /// </summary>
        [Required] public ulong GuildId { get; set; }

    }
}
