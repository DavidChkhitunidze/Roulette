using Microsoft.EntityFrameworkCore.Migrations;

namespace Roulette.Persistance.Migrations
{
    public partial class RefreshTokenForUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshKey",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshKey",
                table: "Users");
        }
    }
}
