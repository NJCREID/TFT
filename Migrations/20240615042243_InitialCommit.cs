using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TFT_API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Augments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InGameKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    IsHidden = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Augments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AugmentStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AugmentStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InGameKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recipe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemStats = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isEmblem = table.Column<bool>(type: "bit", nullable: true),
                    IsComponent = table.Column<bool>(type: "bit", nullable: true),
                    AffectedTraitKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsHidden = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Puuid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Placement = table.Column<int>(type: "int", nullable: false),
                    Augments = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    League = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skill",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartingMana = table.Column<int>(type: "int", nullable: false),
                    SkillMana = table.Column<int>(type: "int", nullable: false),
                    Stats = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Traits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InGameKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TierString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsHidden = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Traits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserGuides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UpVotes = table.Column<int>(type: "int", nullable: false),
                    DownVotes = table.Column<int>(type: "int", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false),
                    Patch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stage2Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stage3Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stage4Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneralDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DifficultyLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayStyle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGuides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    IsUpvote = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Games = table.Column<int>(type: "int", nullable: false),
                    Place = table.Column<int>(type: "int", nullable: false),
                    Top4 = table.Column<int>(type: "int", nullable: false),
                    Win = table.Column<int>(type: "int", nullable: false),
                    Delta = table.Column<double>(type: "float", nullable: false),
                    AugmentStatId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stats_AugmentStats_AugmentStatId",
                        column: x => x.AugmentStatId,
                        principalTable: "AugmentStats",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MatchTrait",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NumUnits = table.Column<int>(type: "int", nullable: false),
                    Style = table.Column<int>(type: "int", nullable: false),
                    TierCurrent = table.Column<int>(type: "int", nullable: false),
                    TierTotal = table.Column<int>(type: "int", nullable: false),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    MatchId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchTrait", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchTrait_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MatchUnit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ItemNames = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rarity = table.Column<int>(type: "int", nullable: false),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    MatchId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchUnit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchUnit_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InGameKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SplashImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CenteredImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecommendedItems = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Health = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttackDamage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DamagePerSecond = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttackRange = table.Column<int>(type: "int", nullable: false),
                    AttackSpeed = table.Column<double>(type: "float", nullable: false),
                    Armor = table.Column<int>(type: "int", nullable: false),
                    MagicalResistance = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    IsHidden = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Units_Skill_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TraitTier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Rarity = table.Column<int>(type: "int", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersistedTraitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraitTier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TraitTier_Traits_PersistedTraitId",
                        column: x => x.PersistedTraitId,
                        principalTable: "Traits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemStats_Stats_StatId",
                        column: x => x.StatId,
                        principalTable: "Stats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StarredUnitStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StarredUnitStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StarredUnitStats_Stats_StatId",
                        column: x => x.StatId,
                        principalTable: "Stats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TraitStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumUnits = table.Column<int>(type: "int", nullable: false),
                    StatId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraitStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TraitStats_Stats_StatId",
                        column: x => x.StatId,
                        principalTable: "Stats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitStats_Stats_StatId",
                        column: x => x.StatId,
                        principalTable: "Stats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AutoGeneratedGuides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InitialUnitId = table.Column<int>(type: "int", nullable: false),
                    UpVotes = table.Column<int>(type: "int", nullable: false),
                    DownVotes = table.Column<int>(type: "int", nullable: false),
                    Views = table.Column<int>(type: "int", nullable: false),
                    Patch = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stage2Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stage3Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stage4Desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneralDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DifficultyLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlayStyle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "UnitTraits",
                columns: table => new
                {
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    TraitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTraits", x => new { x.UnitId, x.TraitId });
                    table.ForeignKey(
                        name: "FK_UnitTraits_Traits_TraitId",
                        column: x => x.TraitId,
                        principalTable: "Traits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitTraits_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuideAugments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AugmentId = table.Column<int>(type: "int", nullable: false),
                    AutoGeneratedGuideId = table.Column<int>(type: "int", nullable: true),
                    UserGuideId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideAugments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuideAugments_Augments_AugmentId",
                        column: x => x.AugmentId,
                        principalTable: "Augments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuideAugments_AutoGeneratedGuides_AutoGeneratedGuideId",
                        column: x => x.AutoGeneratedGuideId,
                        principalTable: "AutoGeneratedGuides",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GuideAugments_UserGuides_UserGuideId",
                        column: x => x.UserGuideId,
                        principalTable: "UserGuides",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GuideTraits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<int>(type: "int", nullable: false),
                    Tier = table.Column<int>(type: "int", nullable: false),
                    TraitId = table.Column<int>(type: "int", nullable: false),
                    AutoGeneratedGuideId = table.Column<int>(type: "int", nullable: true),
                    UserGuideId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuideTraits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuideTraits_AutoGeneratedGuides_AutoGeneratedGuideId",
                        column: x => x.AutoGeneratedGuideId,
                        principalTable: "AutoGeneratedGuides",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GuideTraits_Traits_TraitId",
                        column: x => x.TraitId,
                        principalTable: "Traits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuideTraits_UserGuides_UserGuideId",
                        column: x => x.UserGuideId,
                        principalTable: "UserGuides",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Hexes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    IsStarred = table.Column<bool>(type: "bit", nullable: false),
                    Coordinates = table.Column<int>(type: "int", nullable: false),
                    AutoGeneratedGuideId = table.Column<int>(type: "int", nullable: true),
                    UserGuideId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hexes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hexes_AutoGeneratedGuides_AutoGeneratedGuideId",
                        column: x => x.AutoGeneratedGuideId,
                        principalTable: "AutoGeneratedGuides",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Hexes_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Hexes_UserGuides_UserGuideId",
                        column: x => x.UserGuideId,
                        principalTable: "UserGuides",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HexItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    HexId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HexItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HexItems_Hexes_HexId",
                        column: x => x.HexId,
                        principalTable: "Hexes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HexItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutoGeneratedGuides_InitialUnitId",
                table: "AutoGeneratedGuides",
                column: "InitialUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideAugments_AugmentId",
                table: "GuideAugments",
                column: "AugmentId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideAugments_AutoGeneratedGuideId",
                table: "GuideAugments",
                column: "AutoGeneratedGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideAugments_UserGuideId",
                table: "GuideAugments",
                column: "UserGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideTraits_AutoGeneratedGuideId",
                table: "GuideTraits",
                column: "AutoGeneratedGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideTraits_TraitId",
                table: "GuideTraits",
                column: "TraitId");

            migrationBuilder.CreateIndex(
                name: "IX_GuideTraits_UserGuideId",
                table: "GuideTraits",
                column: "UserGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_Hexes_AutoGeneratedGuideId",
                table: "Hexes",
                column: "AutoGeneratedGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_Hexes_UnitId",
                table: "Hexes",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Hexes_UserGuideId",
                table: "Hexes",
                column: "UserGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_HexItems_HexId",
                table: "HexItems",
                column: "HexId");

            migrationBuilder.CreateIndex(
                name: "IX_HexItems_ItemId",
                table: "HexItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemStats_StatId",
                table: "ItemStats",
                column: "StatId");

            migrationBuilder.CreateIndex(
                name: "IX_Match_Augments",
                table: "Matches",
                column: "Augments");

            migrationBuilder.CreateIndex(
                name: "IX_Match_Placement",
                table: "Matches",
                column: "Placement");

            migrationBuilder.CreateIndex(
                name: "IX_MatchTrait_MatchId",
                table: "MatchTrait",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchTrait_Name",
                table: "MatchTrait",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MatchTrait_NumUnits",
                table: "MatchTrait",
                column: "NumUnits");

            migrationBuilder.CreateIndex(
                name: "IX_MatchUnit_CharacterId",
                table: "MatchUnit",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchUnit_MatchId",
                table: "MatchUnit",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchUnit_Tier",
                table: "MatchUnit",
                column: "Tier");

            migrationBuilder.CreateIndex(
                name: "IX_StarredUnitStats_StatId",
                table: "StarredUnitStats",
                column: "StatId");

            migrationBuilder.CreateIndex(
                name: "IX_Stats_AugmentStatId",
                table: "Stats",
                column: "AugmentStatId");

            migrationBuilder.CreateIndex(
                name: "IX_TraitStats_StatId",
                table: "TraitStats",
                column: "StatId");

            migrationBuilder.CreateIndex(
                name: "IX_TraitTier_PersistedTraitId",
                table: "TraitTier",
                column: "PersistedTraitId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_SkillId",
                table: "Units",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitStats_StatId",
                table: "UnitStats",
                column: "StatId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitTraits_TraitId",
                table: "UnitTraits",
                column: "TraitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuideAugments");

            migrationBuilder.DropTable(
                name: "GuideTraits");

            migrationBuilder.DropTable(
                name: "HexItems");

            migrationBuilder.DropTable(
                name: "ItemStats");

            migrationBuilder.DropTable(
                name: "MatchTrait");

            migrationBuilder.DropTable(
                name: "MatchUnit");

            migrationBuilder.DropTable(
                name: "StarredUnitStats");

            migrationBuilder.DropTable(
                name: "TraitStats");

            migrationBuilder.DropTable(
                name: "TraitTier");

            migrationBuilder.DropTable(
                name: "UnitStats");

            migrationBuilder.DropTable(
                name: "UnitTraits");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "Augments");

            migrationBuilder.DropTable(
                name: "Hexes");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Stats");

            migrationBuilder.DropTable(
                name: "Traits");

            migrationBuilder.DropTable(
                name: "AutoGeneratedGuides");

            migrationBuilder.DropTable(
                name: "UserGuides");

            migrationBuilder.DropTable(
                name: "AugmentStats");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "Skill");
        }
    }
}
