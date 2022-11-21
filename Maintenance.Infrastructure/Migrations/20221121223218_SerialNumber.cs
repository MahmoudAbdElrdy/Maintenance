using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Infrastructure.Migrations
{
    public partial class SerialNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OfficeId",
                table: "RequestComplanit");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "RequestComplanit");

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "RequestComplanit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "RequestComplanit");

            migrationBuilder.AddColumn<long>(
                name: "OfficeId",
                table: "RequestComplanit",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RegionId",
                table: "RequestComplanit",
                type: "bigint",
                nullable: true);
        }
    }
}
