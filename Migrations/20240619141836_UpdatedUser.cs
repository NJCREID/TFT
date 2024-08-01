using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFT_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersistedUserId",
                table: "Votes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserGuideId",
                table: "Votes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommentsCount",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DownVotesCount",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GuidesCount",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpvotesCount",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "UserGuides",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PersistedUserId",
                table: "UserGuides",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GuideId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PersistedUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Users_PersistedUserId",
                        column: x => x.PersistedUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SavedGuides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GuideId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PersistedUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedGuides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedGuides_Users_PersistedUserId",
                        column: x => x.PersistedUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Votes_PersistedUserId",
                table: "Votes",
                column: "PersistedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserGuideId",
                table: "Votes",
                column: "UserGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGuides_PersistedUserId",
                table: "UserGuides",
                column: "PersistedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PersistedUserId",
                table: "Comments",
                column: "PersistedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedGuides_PersistedUserId",
                table: "SavedGuides",
                column: "PersistedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGuides_Users_PersistedUserId",
                table: "UserGuides",
                column: "PersistedUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_UserGuides_UserGuideId",
                table: "Votes",
                column: "UserGuideId",
                principalTable: "UserGuides",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Users_PersistedUserId",
                table: "Votes",
                column: "PersistedUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGuides_Users_PersistedUserId",
                table: "UserGuides");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_UserGuides_UserGuideId",
                table: "Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Users_PersistedUserId",
                table: "Votes");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "SavedGuides");

            migrationBuilder.DropIndex(
                name: "IX_Votes_PersistedUserId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_UserGuideId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_UserGuides_PersistedUserId",
                table: "UserGuides");

            migrationBuilder.DropColumn(
                name: "PersistedUserId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "UserGuideId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "CommentsCount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DownVotesCount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GuidesCount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpvotesCount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "UserGuides");

            migrationBuilder.DropColumn(
                name: "PersistedUserId",
                table: "UserGuides");
        }
    }
}
