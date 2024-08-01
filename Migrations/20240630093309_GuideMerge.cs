﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFT_API.Migrations
{
    /// <inheritdoc />
    public partial class GuideMerge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AutoGeneratedGuides_AutoGeneratedGuideId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_GuideAugments_AutoGeneratedGuides_AutoGeneratedGuideId",
                table: "GuideAugments");

            migrationBuilder.DropForeignKey(
                name: "FK_GuideTraits_AutoGeneratedGuides_AutoGeneratedGuideId",
                table: "GuideTraits");

            migrationBuilder.DropForeignKey(
                name: "FK_Hexes_AutoGeneratedGuides_AutoGeneratedGuideId",
                table: "Hexes");

            migrationBuilder.DropTable(
                name: "AutoGeneratedGuides");

            migrationBuilder.DropIndex(
                name: "IX_Hexes_AutoGeneratedGuideId",
                table: "Hexes");

            migrationBuilder.DropIndex(
                name: "IX_GuideTraits_AutoGeneratedGuideId",
                table: "GuideTraits");

            migrationBuilder.DropIndex(
                name: "IX_GuideAugments_AutoGeneratedGuideId",
                table: "GuideAugments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_AutoGeneratedGuideId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "AutoGeneratedGuideId",
                table: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "UsersUsername",
                table: "UserGuides",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserGuides",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "InitialUnitId",
                table: "UserGuides",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isAutoGenerated",
                table: "UserGuides",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGuides_InitialUnitId",
                table: "UserGuides",
                column: "InitialUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGuides_Units_InitialUnitId",
                table: "UserGuides",
                column: "InitialUnitId",
                principalTable: "Units",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGuides_Units_InitialUnitId",
                table: "UserGuides");

            migrationBuilder.DropIndex(
                name: "IX_UserGuides_InitialUnitId",
                table: "UserGuides");

            migrationBuilder.DropColumn(
                name: "InitialUnitId",
                table: "UserGuides");

            migrationBuilder.DropColumn(
                name: "isAutoGenerated",
                table: "UserGuides");

            migrationBuilder.AlterColumn<string>(
                name: "UsersUsername",
                table: "UserGuides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserGuides",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AutoGeneratedGuideId",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AutoGeneratedGuides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InitialUnitId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DifficultyLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DownVotes = table.Column<int>(type: "int", nullable: false),
                    GeneralDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Patch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayStyle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stage2Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stage3Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stage4Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpVotes = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoGeneratedGuides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutoGeneratedGuides_Units_InitialUnitId",
                        column: x => x.InitialUnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hexes_AutoGeneratedGuideId",
                table: "Hexes",
                column: "AutoGeneratedGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideTraits_AutoGeneratedGuideId",
                table: "GuideTraits",
                column: "AutoGeneratedGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideAugments_AutoGeneratedGuideId",
                table: "GuideAugments",
                column: "AutoGeneratedGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AutoGeneratedGuideId",
                table: "Comments",
                column: "AutoGeneratedGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_AutoGeneratedGuides_InitialUnitId",
                table: "AutoGeneratedGuides",
                column: "InitialUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AutoGeneratedGuides_AutoGeneratedGuideId",
                table: "Comments",
                column: "AutoGeneratedGuideId",
                principalTable: "AutoGeneratedGuides",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GuideAugments_AutoGeneratedGuides_AutoGeneratedGuideId",
                table: "GuideAugments",
                column: "AutoGeneratedGuideId",
                principalTable: "AutoGeneratedGuides",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GuideTraits_AutoGeneratedGuides_AutoGeneratedGuideId",
                table: "GuideTraits",
                column: "AutoGeneratedGuideId",
                principalTable: "AutoGeneratedGuides",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Hexes_AutoGeneratedGuides_AutoGeneratedGuideId",
                table: "Hexes",
                column: "AutoGeneratedGuideId",
                principalTable: "AutoGeneratedGuides",
                principalColumn: "Id");
        }
    }
}
