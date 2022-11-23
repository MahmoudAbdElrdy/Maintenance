using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Infrastructure.Migrations
{
    public partial class NewReueriment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Read",
                table: "Notifications",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<long>(
                name: "ComplanitHistoryId",
                table: "Notifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadDate",
                table: "Notifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApprove",
                table: "ComplanitHistory",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ComplanitHistoryId",
                table: "Notifications",
                column: "ComplanitHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ComplanitHistory_ComplanitHistoryId",
                table: "Notifications",
                column: "ComplanitHistoryId",
                principalTable: "ComplanitHistory",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ComplanitHistory_ComplanitHistoryId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ComplanitHistoryId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ComplanitHistoryId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ReadDate",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsApprove",
                table: "ComplanitHistory");

            migrationBuilder.AlterColumn<bool>(
                name: "Read",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }
    }
}
