using Microsoft.EntityFrameworkCore.Migrations;

namespace Netdrop.Migrations
{
    public partial class Fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedCredentials_AspNetUsers_ApplicationUserId1",
                table: "SavedCredentials");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedCredentials_AspNetUsers_ApplicationUserId1",
                table: "SavedCredentials",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedCredentials_AspNetUsers_ApplicationUserId1",
                table: "SavedCredentials");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedCredentials_AspNetUsers_ApplicationUserId1",
                table: "SavedCredentials",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
