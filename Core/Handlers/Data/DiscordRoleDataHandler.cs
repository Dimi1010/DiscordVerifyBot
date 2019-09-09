using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using DiscordVerifyBot.Resources.Database;
using DiscordVerifyBot.Resources.Database.Model;

namespace DiscordVerifyBot.Core.Handlers
{
    public static class DiscordRoleDataHandler
    {
        public static IList<DiscordRole> GetGuildRoles(ulong guildId)
        {
            using (var DbContext = new SQLiteDatabaseContext())
            {
                return DbContext.DiscordRolesDb.Where(x => x.GuildId == guildId).ToList();
            }
        }

        public static DiscordRole GetGuildRoleById(ulong roleId, ulong guildId)
        {
            using (var DbContext = new SQLiteDatabaseContext())
            {
                return DbContext.DiscordRolesDb.Where(x => x.RoleId == roleId && x.GuildId == guildId).FirstOrDefault();
            }
        }

        public static async Task AddGuildRole(ulong roleId, ulong guildId, DiscordRole.ActionType action)
        {
            using (var DbContext = new SQLiteDatabaseContext())
            {
                var query = DbContext.DiscordRolesDb.Where(x => x.RoleId == roleId && x.GuildId == guildId);
                if (query.Count() < 1)
                {
                    var role = new DiscordRole
                    {
                        RoleId = roleId,
                        GuildId = guildId,
                        Action = action
                    };
                    DbContext.DiscordRolesDb.Add(role);

                    await DbContext.SaveChangesAsync();
                }
                else
                {
                    var role = query.FirstOrDefault();
                    if (role.Action != action)
                    {
                        role.Action = action;

                        DbContext.DiscordRolesDb.Update(role);

                        await DbContext.SaveChangesAsync();
                    }
                }
            }
        }

        public static async Task RemoveGuildRole(ulong roleId, ulong guildId)
        {
            using (var DbContext = new SQLiteDatabaseContext())
            {
                DbContext.DiscordRolesDb.RemoveRange(
                    DbContext.DiscordRolesDb.Where(x =>
                        x.RoleId == roleId &&
                        x.GuildId == guildId
                    ));

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task RemoveGuildRole(DiscordRole role)
        {
            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            using (var DbContext = new SQLiteDatabaseContext())
            {
                DbContext.DiscordRolesDb.RemoveRange(
                    DbContext.DiscordRolesDb.Where(x =>
                        x.RoleId == role.RoleId &&
                        x.GuildId == role.GuildId
                    ));

                await DbContext.SaveChangesAsync();
            }
        }
    }
}
