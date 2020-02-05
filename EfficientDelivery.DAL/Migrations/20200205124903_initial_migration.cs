using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EfficientDelivery.DAL.Migrations
{
    public partial class initial_migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(nullable: false),
                    OrderSource = table.Column<int>(nullable: false),
                    DistanceInfo = table.Column<string>(nullable: true),
                    PublishDate = table.Column<DateTime>(nullable: false),
                    CargoShippingDateInfo = table.Column<string>(nullable: true),
                    TransportType = table.Column<string>(nullable: true),
                    LocationFrom = table.Column<string>(nullable: true),
                    LocationTo = table.Column<string>(nullable: true),
                    CargoInfo = table.Column<string>(nullable: true),
                    PaymentInfo = table.Column<string>(nullable: true),
                    OrderLink = table.Column<string>(nullable: true),
                    OwnerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
