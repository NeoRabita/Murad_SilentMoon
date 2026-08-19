using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilentMoon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addTrackMetadataFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationSeconds",
                table: "Tracks",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "FileSizeBytes",
                table: "Tracks",
                type: "NUMBER(19)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Tracks",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MimeType",
                table: "Tracks",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Narrator",
                table: "Tracks",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "FileSizeBytes",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "MimeType",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "Narrator",
                table: "Tracks");
        }
    }
}
