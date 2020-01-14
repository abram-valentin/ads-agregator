using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AdsAgregator.Backend.Migrations
{
    public partial class AddAdstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ads",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderAdId = table.Column<string>(nullable: true),
                    AdTitle = table.Column<string>(nullable: true),
                    CarInfo = table.Column<string>(nullable: true),
                    ImageLink = table.Column<string>(nullable: true),
                    PriceInfo = table.Column<string>(nullable: true),
                    AdSource = table.Column<int>(nullable: false),
                    AddressInfo = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    CreatedAtInfo = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    AdLink = table.Column<string>(nullable: true),
                    CreatedAt_Internal = table.Column<TimeSpan>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ads_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ads_OwnerId",
                table: "Ads",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ads");
        }
    }
}
