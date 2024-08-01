using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFT_API.Migrations
{
    /// <inheritdoc />
    public partial class StatWrappers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BaseUnitStatId",
                table: "UnitStats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BaseTraitStatId",
                table: "TraitStats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BaseUnitStatId",
                table: "StarredUnitStats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BaseItemStatId",
                table: "ItemStats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BaseAugmentStatId",
                table: "AugmentStats",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BaseAugmentStat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Games = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseAugmentStat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaseItemStat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Games = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseItemStat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaseTraitStat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Games = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseTraitStat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaseUnitStat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Games = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseUnitStat", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitStats_BaseUnitStatId",
                table: "UnitStats",
                column: "BaseUnitStatId");

            migrationBuilder.CreateIndex(
                name: "IX_TraitStats_BaseTraitStatId",
                table: "TraitStats",
                column: "BaseTraitStatId");

            migrationBuilder.CreateIndex(
                name: "IX_StarredUnitStats_BaseUnitStatId",
                table: "StarredUnitStats",
                column: "BaseUnitStatId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemStats_BaseItemStatId",
                table: "ItemStats",
                column: "BaseItemStatId");

            migrationBuilder.CreateIndex(
                name: "IX_AugmentStats_BaseAugmentStatId",
                table: "AugmentStats",
                column: "BaseAugmentStatId");

            migrationBuilder.AddForeignKey(
                name: "FK_AugmentStats_BaseAugmentStat_BaseAugmentStatId",
                table: "AugmentStats",
                column: "BaseAugmentStatId",
                principalTable: "BaseAugmentStat",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemStats_BaseItemStat_BaseItemStatId",
                table: "ItemStats",
                column: "BaseItemStatId",
                principalTable: "BaseItemStat",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StarredUnitStats_BaseUnitStat_BaseUnitStatId",
                table: "StarredUnitStats",
                column: "BaseUnitStatId",
                principalTable: "BaseUnitStat",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TraitStats_BaseTraitStat_BaseTraitStatId",
                table: "TraitStats",
                column: "BaseTraitStatId",
                principalTable: "BaseTraitStat",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UnitStats_BaseUnitStat_BaseUnitStatId",
                table: "UnitStats",
                column: "BaseUnitStatId",
                principalTable: "BaseUnitStat",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AugmentStats_BaseAugmentStat_BaseAugmentStatId",
                table: "AugmentStats");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemStats_BaseItemStat_BaseItemStatId",
                table: "ItemStats");

            migrationBuilder.DropForeignKey(
                name: "FK_StarredUnitStats_BaseUnitStat_BaseUnitStatId",
                table: "StarredUnitStats");

            migrationBuilder.DropForeignKey(
                name: "FK_TraitStats_BaseTraitStat_BaseTraitStatId",
                table: "TraitStats");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitStats_BaseUnitStat_BaseUnitStatId",
                table: "UnitStats");

            migrationBuilder.DropTable(
                name: "BaseAugmentStat");

            migrationBuilder.DropTable(
                name: "BaseItemStat");

            migrationBuilder.DropTable(
                name: "BaseTraitStat");

            migrationBuilder.DropTable(
                name: "BaseUnitStat");

            migrationBuilder.DropIndex(
                name: "IX_UnitStats_BaseUnitStatId",
                table: "UnitStats");

            migrationBuilder.DropIndex(
                name: "IX_TraitStats_BaseTraitStatId",
                table: "TraitStats");

            migrationBuilder.DropIndex(
                name: "IX_StarredUnitStats_BaseUnitStatId",
                table: "StarredUnitStats");

            migrationBuilder.DropIndex(
                name: "IX_ItemStats_BaseItemStatId",
                table: "ItemStats");

            migrationBuilder.DropIndex(
                name: "IX_AugmentStats_BaseAugmentStatId",
                table: "AugmentStats");

            migrationBuilder.DropColumn(
                name: "BaseUnitStatId",
                table: "UnitStats");

            migrationBuilder.DropColumn(
                name: "BaseTraitStatId",
                table: "TraitStats");

            migrationBuilder.DropColumn(
                name: "BaseUnitStatId",
                table: "StarredUnitStats");

            migrationBuilder.DropColumn(
                name: "BaseItemStatId",
                table: "ItemStats");

            migrationBuilder.DropColumn(
                name: "BaseAugmentStatId",
                table: "AugmentStats");
        }
    }
}
