using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.Migrations
{
    public partial class quiz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsUnlocked = table.Column<bool>(nullable: false),
                    NextId = table.Column<string>(nullable: true),
                    Points = table.Column<int>(nullable: false),
                    ThumbnailUrl = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: false),
                    Count = table.Column<int>(nullable: true),
                    RouteId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    Caption = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    DataPath = table.Column<string>(nullable: true),
                    IdForRestApi = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AchievementPendingNotifications",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AchievementId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementPendingNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AchievementPendingNotifications_Achievements_AchievementId",
                        column: x => x.AchievementId,
                        principalTable: "Achievements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Exhibits",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DetailsDataLoaded = table.Column<bool>(nullable: false),
                    IdForRestApi = table.Column<int>(nullable: false),
                    ImageId = table.Column<string>(nullable: true),
                    LastNearbyTime = table.Column<DateTimeOffset>(nullable: true),
                    LocationAsString = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Radius = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    Unlocked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exhibits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exhibits_Media_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pages",
                columns: table => new
                {
                    ImageId = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    AudioId = table.Column<string>(nullable: true),
                    IdForRestApi = table.Column<int>(nullable: false),
                    PageType = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    FontFamily = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    HideYearNumbers = table.Column<bool>(nullable: true),
                    TimeSliderPage_Text = table.Column<string>(nullable: true),
                    TimeSliderPage_Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pages_Media_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pages_Media_AudioId",
                        column: x => x.AudioId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AudioId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DetailsDataLoaded = table.Column<bool>(nullable: false),
                    Distance = table.Column<double>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    IdForRestApi = table.Column<int>(nullable: false),
                    ImageId = table.Column<string>(nullable: true),
                    LastTimeDismissed = table.Column<DateTimeOffset>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Routes_Media_AudioId",
                        column: x => x.AudioId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Routes_Media_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RouteTag",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IdForRestApi = table.Column<int>(nullable: false),
                    ImageId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Tag = table.Column<string>(nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteTag_Media_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExhibitQuizScore",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ExhibitId = table.Column<string>(nullable: true),
                    Score = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExhibitQuizScore", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExhibitQuizScore_Exhibits_ExhibitId",
                        column: x => x.ExhibitId,
                        principalTable: "Exhibits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Quiz",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ExhibitId = table.Column<string>(nullable: true),
                    ImageId = table.Column<string>(nullable: true),
                    OptionA = table.Column<string>(nullable: true),
                    OptionB = table.Column<string>(nullable: true),
                    OptionC = table.Column<string>(nullable: true),
                    OptionD = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quiz", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quiz_Exhibits_ExhibitId",
                        column: x => x.ExhibitId,
                        principalTable: "Exhibits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quiz_Media_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JoinExhibitPage",
                columns: table => new
                {
                    ExhibitId = table.Column<string>(nullable: false),
                    PageId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinExhibitPage", x => new { x.ExhibitId, x.PageId });
                    table.ForeignKey(
                        name: "FK_JoinExhibitPage_Exhibits_ExhibitId",
                        column: x => x.ExhibitId,
                        principalTable: "Exhibits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinExhibitPage_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinPagePage",
                columns: table => new
                {
                    PageId = table.Column<string>(nullable: false),
                    AdditionalInformationPageId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinPagePage", x => new { x.PageId, x.AdditionalInformationPageId });
                    table.ForeignKey(
                        name: "FK_JoinPagePage_Pages_AdditionalInformationPageId",
                        column: x => x.AdditionalInformationPageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinPagePage_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeSliderPageImage",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Date = table.Column<long>(nullable: false),
                    ImageId = table.Column<string>(nullable: true),
                    PageId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSliderPageImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeSliderPageImage_Media_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Media",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimeSliderPageImage_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Waypoint",
                columns: table => new
                {
                    RouteId = table.Column<string>(nullable: false),
                    ExhibitId = table.Column<string>(nullable: false),
                    LocationAsString = table.Column<string>(nullable: true),
                    Visited = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Waypoint", x => new { x.RouteId, x.ExhibitId });
                    table.ForeignKey(
                        name: "FK_Waypoint_Exhibits_ExhibitId",
                        column: x => x.ExhibitId,
                        principalTable: "Exhibits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Waypoint_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinRouteTag",
                columns: table => new
                {
                    RouteId = table.Column<string>(nullable: false),
                    TagId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinRouteTag", x => new { x.RouteId, x.TagId });
                    table.ForeignKey(
                        name: "FK_JoinRouteTag_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinRouteTag_RouteTag_TagId",
                        column: x => x.TagId,
                        principalTable: "RouteTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AchievementPendingNotifications_AchievementId",
                table: "AchievementPendingNotifications",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "IX_ExhibitQuizScore_ExhibitId",
                table: "ExhibitQuizScore",
                column: "ExhibitId");

            migrationBuilder.CreateIndex(
                name: "IX_Exhibits_ImageId",
                table: "Exhibits",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinExhibitPage_PageId",
                table: "JoinExhibitPage",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinPagePage_AdditionalInformationPageId",
                table: "JoinPagePage",
                column: "AdditionalInformationPageId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinRouteTag_TagId",
                table: "JoinRouteTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_ImageId",
                table: "Pages",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_AudioId",
                table: "Pages",
                column: "AudioId");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_ExhibitId",
                table: "Quiz",
                column: "ExhibitId");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_ImageId",
                table: "Quiz",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_AudioId",
                table: "Routes",
                column: "AudioId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_ImageId",
                table: "Routes",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteTag_ImageId",
                table: "RouteTag",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSliderPageImage_ImageId",
                table: "TimeSliderPageImage",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSliderPageImage_PageId",
                table: "TimeSliderPageImage",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_Waypoint_ExhibitId",
                table: "Waypoint",
                column: "ExhibitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AchievementPendingNotifications");

            migrationBuilder.DropTable(
                name: "ExhibitQuizScore");

            migrationBuilder.DropTable(
                name: "JoinExhibitPage");

            migrationBuilder.DropTable(
                name: "JoinPagePage");

            migrationBuilder.DropTable(
                name: "JoinRouteTag");

            migrationBuilder.DropTable(
                name: "Quiz");

            migrationBuilder.DropTable(
                name: "TimeSliderPageImage");

            migrationBuilder.DropTable(
                name: "Waypoint");

            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "RouteTag");

            migrationBuilder.DropTable(
                name: "Pages");

            migrationBuilder.DropTable(
                name: "Exhibits");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "Media");
        }
    }
}
