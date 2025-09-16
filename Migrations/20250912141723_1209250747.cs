using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskList_Server.Migrations
{
    /// <inheritdoc />
    public partial class _1209250747 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TblPermissions",
                table: "TblPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TblCustomers",
                table: "TblCustomers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TblApplications",
                table: "TblApplications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Statuses",
                table: "Statuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Priorities",
                table: "Priorities");

            migrationBuilder.DropColumn(
                name: "testColm",
                table: "TblApplications");

            migrationBuilder.RenameTable(
                name: "TblPermissions",
                newName: "tblPermission");

            migrationBuilder.RenameTable(
                name: "TblCustomers",
                newName: "tblCustomer");

            migrationBuilder.RenameTable(
                name: "TblApplications",
                newName: "tblApplication");

            migrationBuilder.RenameTable(
                name: "Statuses",
                newName: "Status");

            migrationBuilder.RenameTable(
                name: "Priorities",
                newName: "Priority");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Tasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TotalHours",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblPermission",
                table: "tblPermission",
                column: "IntId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblCustomer",
                table: "tblCustomer",
                column: "IntId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tblApplication",
                table: "tblApplication",
                column: "IntId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Status",
                table: "Status",
                column: "StatusId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Priority",
                table: "Priority",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ApplicationId",
                table: "Tasks",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_StatusId",
                table: "Tasks",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId",
                table: "Tasks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Status_StatusId",
                table: "Tasks",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_UserId",
                table: "Tasks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_tblApplication_ApplicationId",
                table: "Tasks",
                column: "ApplicationId",
                principalTable: "tblApplication",
                principalColumn: "IntId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Status_StatusId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_UserId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_tblApplication_ApplicationId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_ApplicationId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_StatusId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_UserId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblPermission",
                table: "tblPermission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblCustomer",
                table: "tblCustomer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tblApplication",
                table: "tblApplication");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Status",
                table: "Status");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Priority",
                table: "Priority");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TotalHours",
                table: "Tasks");

            migrationBuilder.RenameTable(
                name: "tblPermission",
                newName: "TblPermissions");

            migrationBuilder.RenameTable(
                name: "tblCustomer",
                newName: "TblCustomers");

            migrationBuilder.RenameTable(
                name: "tblApplication",
                newName: "TblApplications");

            migrationBuilder.RenameTable(
                name: "Status",
                newName: "Statuses");

            migrationBuilder.RenameTable(
                name: "Priority",
                newName: "Priorities");

            migrationBuilder.AddColumn<string>(
                name: "testColm",
                table: "TblApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TblPermissions",
                table: "TblPermissions",
                column: "IntId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TblCustomers",
                table: "TblCustomers",
                column: "IntId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TblApplications",
                table: "TblApplications",
                column: "IntId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Statuses",
                table: "Statuses",
                column: "StatusId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Priorities",
                table: "Priorities",
                column: "PriorityId");
        }
    }
}
