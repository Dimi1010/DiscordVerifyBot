﻿// <auto-generated />
using System;
using DiscordVerifyBot.Resources.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiscordVerifyBot.Migrations
{
    [DbContext(typeof(SQLiteDatabaseContext))]
    [Migration("20190908100212_v0.1")]
    partial class v01
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("DiscordVerifyBot.Resources.Database.Model.DiscordUser", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("GuildId");

                    b.Property<ulong>("UserId");

                    b.Property<int>("permissionLevel");

                    b.HasKey("Id");

                    b.ToTable("DiscordUsersDB");
                });

            modelBuilder.Entity("DiscordVerifyBot.Resources.Database.Model.VerificationForm", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("ApprovedUtc")
                        .HasColumnType("DateTime");

                    b.Property<ulong?>("Approver");

                    b.Property<ulong>("GuildId");

                    b.Property<bool?>("IsApproved");

                    b.Property<DateTime>("IssuedUtc")
                        .HasColumnType("DateTime");

                    b.Property<ulong>("Verified");

                    b.Property<ulong>("Verifier");

                    b.HasKey("Id");

                    b.ToTable("VerificationDB");
                });
#pragma warning restore 612, 618
        }
    }
}