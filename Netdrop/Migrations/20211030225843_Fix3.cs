using Microsoft.EntityFrameworkCore.Migrations;

namespace Netdrop.Migrations
{
    public partial class Fix3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedCredentials_AspNetUsers_ApplicationUserId1",
                table: "SavedCredentials");

            migrationBuilder.DropIndex(
                name: "IX_SavedCredentials_ApplicationUserId1",
                table: "SavedCredentials");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "SavedCredentials");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "SavedCredentials",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SavedCredentials_ApplicationUserId",
                table: "SavedCredentials",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedCredentials_AspNetUsers_ApplicationUserId",
                table: "SavedCredentials",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavedCredentials_AspNetUsers_ApplicationUserId",
                table: "SavedCredentials");

            migrationBuilder.DropIndex(
                name: "IX_SavedCredentials_ApplicationUserId",
                table: "SavedCredentials");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationUserId",
                table: "SavedCredentials",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "SavedCredentials",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SavedCredentials_ApplicationUserId1",
                table: "SavedCredentials",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedCredentials_AspNetUsers_ApplicationUserId1",
                table: "SavedCredentials",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
