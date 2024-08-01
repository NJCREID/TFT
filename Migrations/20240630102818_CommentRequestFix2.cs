using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFT_API.Migrations
{
    /// <inheritdoc />
    public partial class CommentRequestFix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_PersistedUserId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_PersistedUserId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "PersistedUserId",
                table: "Comments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersistedUserId",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PersistedUserId",
                table: "Comments",
                column: "PersistedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_PersistedUserId",
                table: "Comments",
                column: "PersistedUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
