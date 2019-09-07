using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordVerifyBot.Resources.Database.Model
{
    class VerificationForm
    {
        /// <summary>
        /// ID of the form
        /// </summary>
        [Key] public ulong Id { get; set; }

        /// <summary>
        /// Snowflake ID of the assosiated guild
        /// </summary>
        [Required] public ulong GuildId { get; set; }

        /// <summary>
        /// Snowflake ID of the verified user
        /// </summary>
        [Required] public ulong Verified { get; set; }

        /// <summary>
        /// Snowflake ID of the user performed the verification
        /// </summary>
        [Required] public ulong Verifier { get; set; }

        /// <summary>
        /// Issue date of the verification form in UTC
        /// </summary>
        [Column(TypeName ="DateTime")]
        public DateTime IssuedUtc { get; set; }

        /// <summary>
        /// Snowflake ID of the user approved the verification
        /// </summary>
        public ulong? Approver { get; set; }

        /// <summary>
        /// Boolean if the verification was approved
        /// </summary>
        public bool? IsApproved { get; set; }

        /// <summary>
        /// Verification Approval date in UTC
        /// </summary>
        [Column(TypeName = "DateTime")]
        public DateTime? ApprovedUtc { get; set; }
    }
}
