using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFT_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserGuide : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_UserGuides_UserGuideId",
                table: "Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Users_PersistedUserId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_PersistedUserId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_UserGuideId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "PersistedUserId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "UserGuideId",
                table: "Votes");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDraft",
                table: "UserGuides",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsArchived",
                table: "UserGuides",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "UsersUsername",
                table: "UserGuides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsersUsername",
                table: "UserGuides");

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

            migrationBuilder.AlterColumn<bool>(
                name: "IsDraft",
                table: "UserGuides",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsArchived",
                table: "UserGuides",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_PersistedUserId",
                table: "Votes",
                column: "PersistedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserGuideId",
                table: "Votes",
                column: "UserGuideId");

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
    }
}
