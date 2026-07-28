using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilentMoon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addTrack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ContentId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Title = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Duration = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    AudioUrl = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SortOrder = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tracks");
        }
    }
}
