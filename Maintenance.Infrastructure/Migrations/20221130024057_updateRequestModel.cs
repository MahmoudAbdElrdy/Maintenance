using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Infrastructure.Migrations
{
    public partial class updateRequestModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicantName",
                table: "RequestComplanit");

            migrationBuilder.DropColumn(
                name: "ApplicantNationalId",
                table: "RequestComplanit");

            migrationBuilder.DropColumn(
                name: "ApplicantPhoneNumber",
                table: "RequestComplanit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicantName",
                table: "RequestComplanit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicantNationalId",
                table: "RequestComplanit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicantPhoneNumber",
                table: "RequestComplanit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
