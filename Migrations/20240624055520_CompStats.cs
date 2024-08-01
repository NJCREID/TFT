using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFT_API.Migrations
{
    /// <inheritdoc />
    public partial class CompStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompStatId",
                table: "Units",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BaseCompStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Games = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseCompStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompStat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatId = table.Column<int>(type: "int", nullable: false),
                    BaseCompStatId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompStat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompStat_BaseCompStats_BaseCompStatId",
                        column: x => x.BaseCompStatId,
                        principalTable: "BaseCompStats",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompStat_Stats_StatId",
                        column: x => x.StatId,
                        principalTable: "Stats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Units_CompStatId",
                table: "Units",
                column: "CompStatId");

            migrationBuilder.CreateIndex(
                name: "IX_CompStat_BaseCompStatId",
                table: "CompStat",
                column: "BaseCompStatId");

            migrationBuilder.CreateIndex(
                name: "IX_CompStat_StatId",
                table: "CompStat",
                column: "StatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Units_CompStat_CompStatId",
                table: "Units",
                column: "CompStatId",
                principalTable: "CompStat",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_CompStat_CompStatId",
                table: "Units");

            migrationBuilder.DropTable(
                name: "CompStat");

            migrationBuilder.DropTable(
                name: "BaseCompStats");

            migrationBuilder.DropIndex(
                name: "IX_Units_CompStatId",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "CompStatId",
                table: "Units");
        }
    }
}
