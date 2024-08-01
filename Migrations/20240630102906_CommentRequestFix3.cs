using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFT_API.Migrations
{
    /// <inheritdoc />
    public partial class CommentRequestFix3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_UserGuides_UserGuideId",
                table: "Comments");

            migrationBuilder.AlterColumn<int>(
                name: "UserGuideId",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_UserGuides_UserGuideId",
                table: "Comments",
                column: "UserGuideId",
                principalTable: "UserGuides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_UserGuides_UserGuideId",
                table: "Comments");

            migrationBuilder.AlterColumn<int>(
                name: "UserGuideId",
                table: "Comments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_UserGuides_UserGuideId",
                table: "Comments",
                column: "UserGuideId",
                principalTable: "UserGuides",
                principalColumn: "Id");
        }
    }
}
