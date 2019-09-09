using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace DiscordVerifyBot.Resources.Database.Model
{
    public class DiscordRole
    {
        public enum ActionType
        {
            None,
            Add,
            Remove
        }

        [Key] public ulong RoleId { get; set; }

        [Required] public ulong GuildId { get; set; }

        [Required] public ActionType Action { get; set; }
    }
}
