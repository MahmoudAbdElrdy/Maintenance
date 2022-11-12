using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Infrastructure.Migrations
{
    public partial class Report1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "RequestReport");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "RequestReport");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "RequestReport");

            migrationBuilder.RenameColumn(
                name: "NameEn",
                table: "RequestReport",
                newName: "Description");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "RequestReport",
                newName: "NameEn");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "RequestReport",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "RequestReport",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "RequestReport",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
