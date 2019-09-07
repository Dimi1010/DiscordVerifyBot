using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordVerifyBot.Migrations
{
    public partial class v01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "L1UsersDB",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<ulong>(nullable: false),
                    GuildId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L1UsersDB", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "L2UsersDB",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<ulong>(nullable: false),
                    GuildId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_L2UsersDB", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VerificationDB",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildId = table.Column<ulong>(nullable: false),
                    Verified = table.Column<ulong>(nullable: false),
                    Verifier = table.Column<ulong>(nullable: false),
                    IssuedUtc = table.Column<DateTime>(type: "DateTime", nullable: false),
                    Approver = table.Column<ulong>(nullable: true),
                    IsApproved = table.Column<bool>(nullable: true),
                    ApprovedUtc = table.Column<DateTime>(type: "DateTime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationDB", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "L1UsersDB");

            migrationBuilder.DropTable(
                name: "L2UsersDB");

            migrationBuilder.DropTable(
                name: "VerificationDB");
        }
    }
}
