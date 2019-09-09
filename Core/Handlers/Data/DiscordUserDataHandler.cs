using System.Linq;
using System.Threading.Tasks;

using DiscordVerifyBot.Resources.Database;
using DiscordVerifyBot.Resources.Database.Model;

namespace DiscordVerifyBot.Core.Handlers
{
    public static class DiscordUserDataHandler
    {
        public static IQueryable<DiscordGuildUser> GetGuildUsers(ulong GuildId)
        {
            using (var DbContext = new SQLiteDatabaseContext())
            {
                return DbContext.DiscordUsersDB.Where(x => x.GuildId == GuildId);
            }
        }

        public static DiscordGuildUser GetGuildUserById(ulong UserId, ulong GuildId)
        {
            using (var DbContext = new SQLiteDatabaseContext())
            {
                return DbContext.DiscordUsersDB.Where(x => x.UserId == UserId && x.GuildId == GuildId).FirstOrDefault();
            }
        }

        public static async Task AddGuildUser(ulong UserId, ulong GuildId, DiscordGuildUser.PermissionLevels permission)
        {
            using (var DbContext = new SQLiteDatabaseContext())
            {
                var query = DbContext.DiscordUsersDB.Where(x => x.UserId == UserId && x.GuildId == GuildId);
                if (query.Count() < 1)
                {
                    var user = new DiscordGuildUser
                    {
                        UserId = UserId,
                        GuildId = GuildId,
                        PermissionLevel = permission
                    };
                    DbContext.DiscordUsersDB.Add(user);

                    await DbContext.SaveChangesAsync();
                }
                else
                {
                    var user = query.FirstOrDefault();
                    if (user.PermissionLevel != permission)
                    {
                        user.PermissionLevel = permission;

                        DbContext.DiscordUsersDB.Update(user);

                        await DbContext.SaveChangesAsync();
                    }
                }
            }
        }

        public static async Task RemoveGuildUser(ulong UserId, ulong GuildId)
        {
            using (var DbContext = new SQLiteDatabaseContext())
            {
                DbContext.DiscordUsersDB.RemoveRange(
                    DbContext.DiscordUsersDB.Where(x =>
                        x.UserId == UserId &&
                        x.GuildId == GuildId
                    ));

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task RemoveGuildUser(DiscordGuildUser user)
        {
            using (var DbContext = new SQLiteDatabaseContext())
            {
                DbContext.DiscordUsersDB.RemoveRange(
                    DbContext.DiscordUsersDB.Where(x =>
                        x.UserId == user.UserId &&
                        x.GuildId == user.GuildId
                    ));

                await DbContext.SaveChangesAsync();
            }
        }
    }
}
