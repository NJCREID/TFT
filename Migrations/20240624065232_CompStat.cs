using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFT_API.Migrations
{
    /// <inheritdoc />
    public partial class CompStat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompStat_BaseCompStats_BaseCompStatId",
                table: "CompStat");

            migrationBuilder.DropForeignKey(
                name: "FK_CompStat_Stats_StatId",
                table: "CompStat");

            migrationBuilder.DropForeignKey(
                name: "FK_Units_CompStat_CompStatId",
                table: "Units");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompStat",
                table: "CompStat");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BaseCompStats",
                table: "BaseCompStats");

            migrationBuilder.RenameTable(
                name: "CompStat",
                newName: "CompStats");

            migrationBuilder.RenameTable(
                name: "BaseCompStats",
                newName: "BaseCompStat");

            migrationBuilder.RenameIndex(
                name: "IX_CompStat_StatId",
                table: "CompStats",
                newName: "IX_CompStats_StatId");

            migrationBuilder.RenameIndex(
                name: "IX_CompStat_BaseCompStatId",
                table: "CompStats",
                newName: "IX_CompStats_BaseCompStatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompStats",
                table: "CompStats",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BaseCompStat",
                table: "BaseCompStat",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompStats_BaseCompStat_BaseCompStatId",
                table: "CompStats",
                column: "BaseCompStatId",
                principalTable: "BaseCompStat",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompStats_Stats_StatId",
                table: "CompStats",
                column: "StatId",
                principalTable: "Stats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Units_CompStats_CompStatId",
                table: "Units",
                column: "CompStatId",
                principalTable: "CompStats",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompStats_BaseCompStat_BaseCompStatId",
                table: "CompStats");

            migrationBuilder.DropForeignKey(
                name: "FK_CompStats_Stats_StatId",
                table: "CompStats");

            migrationBuilder.DropForeignKey(
                name: "FK_Units_CompStats_CompStatId",
                table: "Units");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompStats",
                table: "CompStats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BaseCompStat",
                table: "BaseCompStat");

            migrationBuilder.RenameTable(
                name: "CompStats",
                newName: "CompStat");

            migrationBuilder.RenameTable(
                name: "BaseCompStat",
                newName: "BaseCompStats");

            migrationBuilder.RenameIndex(
                name: "IX_CompStats_StatId",
                table: "CompStat",
                newName: "IX_CompStat_StatId");

            migrationBuilder.RenameIndex(
                name: "IX_CompStats_BaseCompStatId",
                table: "CompStat",
                newName: "IX_CompStat_BaseCompStatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompStat",
                table: "CompStat",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BaseCompStats",
                table: "BaseCompStats",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompStat_BaseCompStats_BaseCompStatId",
                table: "CompStat",
                column: "BaseCompStatId",
                principalTable: "BaseCompStats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompStat_Stats_StatId",
                table: "CompStat",
                column: "StatId",
                principalTable: "Stats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Units_CompStat_CompStatId",
                table: "Units",
                column: "CompStatId",
                principalTable: "CompStat",
                principalColumn: "Id");
        }
    }
}
