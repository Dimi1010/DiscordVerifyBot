using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordVerifyBot.Migrations
{
    public partial class v02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                name: "Id",
                table: "DiscordUsersDB",
                nullable: false,
                oldClrType: typeof(ulong))
                .Annotation("Sqlite:Autoincrement", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<ulong>(
                name: "Id",
                table: "DiscordUsersDB",
                nullable: false,
                oldClrType: typeof(ulong))
                .OldAnnotation("Sqlite:Autoincrement", true);
        }
    }
}
