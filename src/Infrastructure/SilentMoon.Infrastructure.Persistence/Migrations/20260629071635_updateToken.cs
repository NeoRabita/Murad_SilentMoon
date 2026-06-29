using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilentMoon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUsers_RefreshTokens_RefreshTokenId",
                table: "ApplicationUsers");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUsers_RefreshTokenId",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenId",
                table: "ApplicationUsers");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "RefreshTokens",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ApplicationUserId",
                table: "RefreshTokens",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_ApplicationUsers_ApplicationUserId",
                table: "RefreshTokens",
                column: "ApplicationUserId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_ApplicationUsers_ApplicationUserId",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_ApplicationUserId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "RefreshTokens");

            migrationBuilder.AddColumn<int>(
                name: "RefreshTokenId",
                table: "ApplicationUsers",
                type: "NUMBER(10)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_RefreshTokenId",
                table: "ApplicationUsers",
                column: "RefreshTokenId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUsers_RefreshTokens_RefreshTokenId",
                table: "ApplicationUsers",
                column: "RefreshTokenId",
                principalTable: "RefreshTokens",
                principalColumn: "Id");
        }
    }
}
