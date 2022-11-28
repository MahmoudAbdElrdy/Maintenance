using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Infrastructure.Migrations
{
    public partial class RequestComplanitNotfications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "RequestComplanit",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

            migrationBuilder.AddColumn<long>(
                name: "OfficeId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RegionId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ComplanitFilters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    OfficeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryComplanitId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplanitFilters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComplanitFilters_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComplanitFilters_CreatedBy",
                table: "ComplanitFilters",
                column: "CreatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComplanitFilters");

            migrationBuilder.DropColumn(
                name: "OfficeId",
                table: "RequestComplanit");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "RequestComplanit");

            migrationBuilder.DropColumn(
                name: "OfficeId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "RequestComplanit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RoomId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
