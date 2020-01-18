using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AdsAgregator.Backend.Migrations
{
    public partial class RemoveCreatedAt_Internalfieldfromadstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt_Internal",
                table: "Ads");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "CreatedAt_Internal",
                table: "Ads",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
