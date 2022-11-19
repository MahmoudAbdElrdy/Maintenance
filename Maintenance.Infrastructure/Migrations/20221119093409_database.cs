using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maintenance.Infrastructure.Migrations
{
    public partial class database : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachmentComplanitHistory_ComplanitHistory_ComplanitHistoryId",
                table: "AttachmentComplanitHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_CheckListRequest_CheckListsComplanit_CheckListComplanitId",
                table: "CheckListRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_ComplanitHistory_RequestComplanit_RequestComplanitId",
                table: "ComplanitHistory");

            migrationBuilder.AlterColumn<long>(
                name: "RequestComplanitId",
                table: "ComplanitHistory",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CheckListComplanitId",
                table: "CheckListRequest",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ComplanitHistoryId",
                table: "AttachmentComplanitHistory",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AttachmentComplanitHistory_ComplanitHistory_ComplanitHistoryId",
                table: "AttachmentComplanitHistory",
                column: "ComplanitHistoryId",
                principalTable: "ComplanitHistory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CheckListRequest_CheckListsComplanit_CheckListComplanitId",
                table: "CheckListRequest",
                column: "CheckListComplanitId",
                principalTable: "CheckListsComplanit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ComplanitHistory_RequestComplanit_RequestComplanitId",
                table: "ComplanitHistory",
                column: "RequestComplanitId",
                principalTable: "RequestComplanit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachmentComplanitHistory_ComplanitHistory_ComplanitHistoryId",
                table: "AttachmentComplanitHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_CheckListRequest_CheckListsComplanit_CheckListComplanitId",
                table: "CheckListRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_ComplanitHistory_RequestComplanit_RequestComplanitId",
                table: "ComplanitHistory");

            migrationBuilder.AlterColumn<long>(
                name: "RequestComplanitId",
                table: "ComplanitHistory",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "CheckListComplanitId",
                table: "CheckListRequest",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ComplanitHistoryId",
                table: "AttachmentComplanitHistory",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachmentComplanitHistory_ComplanitHistory_ComplanitHistoryId",
                table: "AttachmentComplanitHistory",
                column: "ComplanitHistoryId",
                principalTable: "ComplanitHistory",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckListRequest_CheckListsComplanit_CheckListComplanitId",
                table: "CheckListRequest",
                column: "CheckListComplanitId",
                principalTable: "CheckListsComplanit",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ComplanitHistory_RequestComplanit_RequestComplanitId",
                table: "ComplanitHistory",
                column: "RequestComplanitId",
                principalTable: "RequestComplanit",
                principalColumn: "Id");
        }
    }
}
