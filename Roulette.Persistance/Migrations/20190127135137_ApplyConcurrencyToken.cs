using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Roulette.Persistance.Migrations
{
    public partial class ApplyConcurrencyToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Jackpots",
                rowVersion: true,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Jackpots");
        }
    }
}
