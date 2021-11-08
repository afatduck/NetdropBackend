using Microsoft.EntityFrameworkCore.Migrations;

namespace Netdrop.Migrations
{
    public partial class port : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Port",
                table: "SavedCredentials",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Port",
                table: "SavedCredentials");
        }
    }
}
