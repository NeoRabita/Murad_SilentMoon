using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilentMoon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addHomeFeedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationSeconds",
                table: "Contents",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPopular",
                table: "Contents",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Subtitle",
                table: "Contents",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContentNarrators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ContentId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Gender = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentNarrators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentNarrators_Contents_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Contents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentNarrators_ContentId",
                table: "ContentNarrators",
                column: "ContentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentNarrators");

            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "IsPopular",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "Subtitle",
                table: "Contents");
        }
    }
}
