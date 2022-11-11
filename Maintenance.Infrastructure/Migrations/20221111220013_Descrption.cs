using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Infrastructure.Migrations
{
    public partial class Descrption : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "CheckListsReport",
                newName: "DescriptionEn");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "CategoriesReport",
                newName: "DescriptionEn");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "CheckListsReport",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "CategoriesReport",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "CheckListsReport");

            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "CategoriesReport");

            migrationBuilder.RenameColumn(
                name: "DescriptionEn",
                table: "CheckListsReport",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "DescriptionEn",
                table: "CategoriesReport",
                newName: "Description");
        }
    }
}
