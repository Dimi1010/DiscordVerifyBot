using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using DiscordVerifyBot.Resources.Database.Model;


namespace DiscordVerifyBot.Resources.Database
{
    class SQLiteDatabaseContext : DbContext
    {
        /// <summary>
        /// Database of the users allowed to Verify
        /// </summary>
        public DbSet<DiscordGuildUser> DiscordUsersDB { get; set; }

        /// <summary>
        /// Discord Role database
        /// </summary>
        public DbSet<DiscordRole> DiscordRolesDb { get; set; }

        /// <summary>
        /// Verification Database
        /// </summary>
        public DbSet<VerificationForm> VerificationDB { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder Options)
        {
            string AssemblyFullPath = Assembly.GetEntryAssembly().Location;
            string AssemblyFilename = Path.GetFileName(AssemblyFullPath);

            //Path to the Database
            string DbPath = "";
            //HACK: Comment following line when Updating DB from NuGet
            DbPath = AssemblyFullPath.Replace(AssemblyFilename, @"Data\");
            //Name of the Database
            string DbFilename = "Database.sqlite";

            Options.UseSqlite($"Data Source={DbPath}{DbFilename}");
        }
    }
}
