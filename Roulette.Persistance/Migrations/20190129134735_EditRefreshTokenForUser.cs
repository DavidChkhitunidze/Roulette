using Microsoft.EntityFrameworkCore.Migrations;

namespace Roulette.Persistance.Migrations
{
    public partial class EditRefreshTokenForUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshKey",
                table: "Users",
                newName: "RefreshToken");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                table: "Users",
                newName: "RefreshKey");
        }
    }
}
