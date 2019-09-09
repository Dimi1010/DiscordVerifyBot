using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;

using DiscordVerifyBot.Resources.Database;
using DiscordVerifyBot.Resources.Database.Model;

namespace DiscordVerifyBot.Core.Handlers
{
    class VerificationFormDataHandler
    {
        /// <summary>
        /// Returns all verification forms for a guild
        /// </summary>
        /// <param name="guildId">Snowflake ID of the guild</param>
        /// <returns></returns>
        public static IList<VerificationForm> GetVerificationFormsByGuild(ulong guildId)
        {
            using (var DbContext = new SQLiteDatabaseContext())
            {
                return DbContext.VerificationDB.Where(x => x.GuildId == guildId).ToList();
            }
        }

        /// <summary>
        /// Returns the pending verification forms for a guild
        /// </summary>
        /// <param name="guildId">Snowflake ID of the guild</param>
        /// <returns></returns>
        public static IList<VerificationForm> GetPendingVerificationFormsByGuild(ulong guildId)
        {
            using (var DbContext = new SQLiteDatabaseContext())
            {
                return DbContext.VerificationDB.Where(x => x.GuildId == guildId && x.IsApproved == null).ToList();
            }
        }

        /// <summary>
        /// Adds new pending verification form record
        /// </summary>
        /// <param name="guildId">Snowflake ID of the guild</param>
        /// <param name="verified">Snowflake ID of the verified user</param>
        /// <param name="verifier">Snowflake ID of the verifier</param>
        /// <returns></returns>
        /// <exception cref="VerificationFormExistsException"></exception>
        public static async Task AddPendingVerificationForm(ulong guildId, ulong verified, ulong verifier)
        {
            using (var DbContext = new SQLiteDatabaseContext())
            {
                var query = DbContext.VerificationDB.Where(x =>
                    x.GuildId == guildId &&
                    x.Verified == verified &&
                    x.IsApproved == null
                );

                if(query.Count() < 1)
                {
                    var form = new VerificationForm
                    {
                        GuildId = guildId,
                        Verified = verified,
                        Verifier = verifier,
                        IssuedUtc = DateTime.UtcNow
                    };

                    DbContext.VerificationDB.Add(form);

                    await DbContext.SaveChangesAsync();
                }
                else
                {
                    throw new VerificationFormExistsException();
                }
            }
        }

        /// <summary>
        /// Adds new pending verification form record
        /// </summary>
        /// <param name="form">Verification form</param>
        /// <returns></returns>
        public static async Task AddPendingVerificationForm(VerificationForm form)
        {
            using (var DbContext = new SQLiteDatabaseContext())
            {
                var query = DbContext.VerificationDB.Where(x =>
                    x.GuildId == form.GuildId &&
                    x.Verified == form.Verified &&
                    x.IsApproved == null
                );

                if (query.Count() < 1)
                {
                    var submit = new VerificationForm
                    {
                        GuildId = form.GuildId,
                        Verified = form.Verified,
                        Verifier = form.Verifier,
                        IssuedUtc = DateTime.UtcNow
                    };

                    DbContext.VerificationDB.Add(submit);

                    await DbContext.SaveChangesAsync();
                }
            }
        }


        /// <summary>
        /// Adds new Verification form record if a pending record does not exist or updates the existing pending record
        /// </summary>
        /// <param name="form">Verification Form record</param>
        /// <returns></returns>
        public static async Task AddFullVerificationForm(VerificationForm form)
        {
            using (var DbContext = new SQLiteDatabaseContext())
            {
                var query = DbContext.VerificationDB.Where(x =>
                    x.GuildId == form.GuildId &&
                    x.Verified == form.Verified &&
                    x.IsApproved == null
                );

                if (query.Count() < 1)
                {
                    if(form.IssuedUtc == null)
                    {
                        form.IssuedUtc = DateTime.UtcNow;
                    }

                    if (form.ApprovedUtc == null)
                    {
                        form.ApprovedUtc = DateTime.UtcNow;
                    }

                    DbContext.VerificationDB.Add(form);

                    await DbContext.SaveChangesAsync();
                }
                else
                {
                    var qForm = query.First();

                    if(form.IsApproved != null)
                    {
                        qForm.IsApproved = form.IsApproved;
                        qForm.Approver = form.Approver;
                        qForm.ApprovedUtc = form.ApprovedUtc ?? DateTime.UtcNow;

                        DbContext.VerificationDB.Update(qForm);

                        await DbContext.SaveChangesAsync();
                    }
                }
            }
        }
    }

    #region Exceptions

    public class VerificationFormException : Exception
    {
        public VerificationFormException()
        {
        }

        public VerificationFormException(string message) : base(message)
        {
        }

        public VerificationFormException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected VerificationFormException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class VerificationFormExistsException : VerificationFormException
    {
        public VerificationFormExistsException()
        {
        }

        public VerificationFormExistsException(string message) : base(message)
        {
        }

        public VerificationFormExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected VerificationFormExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    #endregion
}
