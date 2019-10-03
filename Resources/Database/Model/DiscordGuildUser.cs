using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordVerifyBot.Resources.Database.Model
{
    public class DiscordGuildUser
    {
        public enum PermissionLevels
        {
            None,
            Verify,
            Approve
        }

        /// <summary>
        /// ID of the data entry
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public ulong Id { get; set; }

        /// <summary>
        /// Snowflake ID of the discord user
        /// </summary>
        [Required] public ulong UserId { get; set; }

        /// <summary>
        /// Snowflake Id of the assosiated guild
        /// </summary>
        [Required] public ulong GuildId { get; set; }

        [Required] public PermissionLevels PermissionLevel { get; set; }
    }
}
