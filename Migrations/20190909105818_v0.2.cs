using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordVerifyBot.Migrations
{
    public partial class v02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "permissionLevel",
                table: "DiscordUsersDB",
                newName: "PermissionLevel");

            migrationBuilder.CreateTable(
                name: "DiscordRolesDb",
                columns: table => new
                {
                    RoleId = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildId = table.Column<ulong>(nullable: false),
                    Action = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordRolesDb", x => x.RoleId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscordRolesDb");

            migrationBuilder.RenameColumn(
                name: "PermissionLevel",
                table: "DiscordUsersDB",
                newName: "permissionLevel");
        }
    }
}
