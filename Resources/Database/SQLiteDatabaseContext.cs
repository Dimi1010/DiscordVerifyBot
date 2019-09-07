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
        public DbSet<L1GuildUser> L1UsersDB { get; set; }
        
        /// <summary>
        /// Database of the users allowed to Approve verifications
        /// </summary>
        public DbSet<L2GuildUser> L2UsersDB { get; set; }

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
            //HACK: Comment when Updating DB
            DbPath = AssemblyFullPath.Replace(AssemblyFilename, @"Data\");
            //Name of the Database
            string DbFilename = "Database.sqlite";

            Options.UseSqlite($"Data Source={DbPath}{DbFilename}");
        }
    }
}
