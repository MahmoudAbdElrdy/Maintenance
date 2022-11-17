using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Infrastructure.Migrations
{
    public partial class ComplanitHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComplanitHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    ComplanitStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    RequestComplanitId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplanitHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComplanitHistory_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ComplanitHistory_RequestComplanit_RequestComplanitId",
                        column: x => x.RequestComplanitId,
                        principalTable: "RequestComplanit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AttachmentComplanitHistory",
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
                    ComplanitHistoryId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentComplanitHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttachmentComplanitHistory_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttachmentComplanitHistory_ComplanitHistory_ComplanitHistoryId",
                        column: x => x.ComplanitHistoryId,
                        principalTable: "ComplanitHistory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentComplanitHistory_ComplanitHistoryId",
                table: "AttachmentComplanitHistory",
                column: "ComplanitHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentComplanitHistory_CreatedBy",
                table: "AttachmentComplanitHistory",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ComplanitHistory_CreatedBy",
                table: "ComplanitHistory",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ComplanitHistory_RequestComplanitId",
                table: "ComplanitHistory",
                column: "RequestComplanitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttachmentComplanitHistory");

            migrationBuilder.DropTable(
                name: "ComplanitHistory");
        }
    }
}
