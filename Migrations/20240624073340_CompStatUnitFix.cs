using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFT_API.Migrations
{
    /// <inheritdoc />
    public partial class CompStatUnitFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_CompStats_CompStatId",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Units_CompStatId",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "CompStatId",
                table: "Units");

            migrationBuilder.CreateTable(
                name: "CompStatUnit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompStatId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompStatUnit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompStatUnit_CompStats_CompStatId",
                        column: x => x.CompStatId,
                        principalTable: "CompStats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompStatUnit_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompStatUnit_CompStatId",
                table: "CompStatUnit",
                column: "CompStatId");

            migrationBuilder.CreateIndex(
                name: "IX_CompStatUnit_UnitId",
                table: "CompStatUnit",
                column: "UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompStatUnit");

            migrationBuilder.AddColumn<int>(
                name: "CompStatId",
                table: "Units",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Units_CompStatId",
                table: "Units",
                column: "CompStatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Units_CompStats_CompStatId",
                table: "Units",
                column: "CompStatId",
                principalTable: "CompStats",
                principalColumn: "Id");
        }
    }
}
