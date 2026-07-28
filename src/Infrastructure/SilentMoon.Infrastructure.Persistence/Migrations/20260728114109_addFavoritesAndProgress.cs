using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilentMoon.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addFavoritesAndProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentTopics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ContentId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TopicId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentTopics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentTopics_Contents_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Contents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentTopics_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ApplicationUserId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ContentId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favorites_ApplicationUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorites_Contents_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Contents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaybackProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ApplicationUserId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TrackId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PositionSeconds = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaybackProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaybackProgresses_ApplicationUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaybackProgresses_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Tracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentTopics_ContentId",
                table: "ContentTopics",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTopics_TopicId",
                table: "ContentTopics",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_ApplicationUserId",
                table: "Favorites",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_ContentId",
                table: "Favorites",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaybackProgresses_ApplicationUserId",
                table: "PlaybackProgresses",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaybackProgresses_TrackId",
                table: "PlaybackProgresses",
                column: "TrackId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentTopics");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "PlaybackProgresses");
        }
    }
}
