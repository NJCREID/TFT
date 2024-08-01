using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFT_API.Migrations
{
    /// <inheritdoc />
    public partial class LeagueRequiredFalse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpvotesCount",
                table: "Users",
                newName: "UpVotesCount");

            migrationBuilder.AlterColumn<string>(
                name: "League",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpVotesCount",
                table: "Users",
                newName: "UpvotesCount");

            migrationBuilder.AlterColumn<string>(
                name: "League",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
