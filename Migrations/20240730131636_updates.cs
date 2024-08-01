using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFT_API.Migrations
{
    /// <inheritdoc />
    public partial class updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_UserId",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "PostId",
                table: "Votes",
                newName: "UserGuideId");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserGuideId",
                table: "Votes",
                column: "UserGuideId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_UserGuides_UserGuideId",
                table: "Votes",
                column: "UserGuideId",
                principalTable: "UserGuides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_UserGuides_UserGuideId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_UserGuideId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "Author",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "UserGuideId",
                table: "Votes",
                newName: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
