using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Infrastructure.Migrations
{
    public partial class Report : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestReport",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestReport_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AttachmentReport",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RequestReportId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttachmentReport_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttachmentReport_RequestReport_RequestReportId",
                        column: x => x.RequestReportId,
                        principalTable: "RequestReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckListRequest",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RequestReportId = table.Column<long>(type: "bigint", nullable: false),
                    CheckListReportId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckListRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckListRequest_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CheckListRequest_CheckListsReport_CheckListReportId",
                        column: x => x.CheckListReportId,
                        principalTable: "CheckListsReport",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CheckListRequest_RequestReport_RequestReportId",
                        column: x => x.RequestReportId,
                        principalTable: "RequestReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentReport_CreatedBy",
                table: "AttachmentReport",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentReport_RequestReportId",
                table: "AttachmentReport",
                column: "RequestReportId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckListRequest_CheckListReportId",
                table: "CheckListRequest",
                column: "CheckListReportId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckListRequest_CreatedBy",
                table: "CheckListRequest",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CheckListRequest_RequestReportId",
                table: "CheckListRequest",
                column: "RequestReportId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestReport_CreatedBy",
                table: "RequestReport",
                column: "CreatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttachmentReport");

            migrationBuilder.DropTable(
                name: "CheckListRequest");

            migrationBuilder.DropTable(
                name: "RequestReport");
        }
    }
}
