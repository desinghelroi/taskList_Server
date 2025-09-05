using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskList_Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Priority",
                columns: table => new
                {
                    PriorityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Priority", x => x.PriorityId);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    StatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.StatusId);
                });

            migrationBuilder.CreateTable(
                name: "TasksOlds",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    PriorityId = table.Column<int>(type: "int", nullable: false),
                    ApplicationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DelegatedTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Visible = table.Column<bool>(type: "bit", nullable: false),
                    SeriousBug = table.Column<bool>(type: "bit", nullable: false),
                    SmallBug = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TasksOlds", x => x.TaskId);
                });

            migrationBuilder.CreateTable(
                name: "TblAdmins",
                columns: table => new
                {
                    IntId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChrFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChrLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChrUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChrPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DtLastLogin = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblAdmins", x => x.IntId);
                });

            migrationBuilder.CreateTable(
                name: "tblApplication",
                columns: table => new
                {
                    IntId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IntCustomerId = table.Column<int>(type: "int", nullable: true),
                    ChrApplicationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    testColm = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblApplication", x => x.IntId);
                });

            migrationBuilder.CreateTable(
                name: "tblCustomer",
                columns: table => new
                {
                    IntId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChrCustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChrCustomerCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCustomer", x => x.IntId);
                });

            migrationBuilder.CreateTable(
                name: "tblPermission",
                columns: table => new
                {
                    IntId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChrPermission = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPermission", x => x.IntId);
                });

            migrationBuilder.CreateTable(
                name: "TblUploadedFiles",
                columns: table => new
                {
                    IntId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IntTaskId = table.Column<int>(type: "int", nullable: false),
                    ChrOriginalFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChrSavedFileName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblUploadedFiles", x => x.IntId);
                });

            migrationBuilder.CreateTable(
                name: "TraceMarches",
                columns: table => new
                {
                    RowNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventClass = table.Column<int>(type: "int", nullable: true),
                    TextData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NtuserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoginName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cpu = table.Column<int>(type: "int", nullable: true),
                    Reads = table.Column<long>(type: "bigint", nullable: true),
                    Writes = table.Column<long>(type: "bigint", nullable: true),
                    Duration = table.Column<long>(type: "bigint", nullable: true),
                    ClientProcessId = table.Column<int>(type: "int", nullable: true),
                    Spid = table.Column<int>(type: "int", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BinaryData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    DatabaseId = table.Column<int>(type: "int", nullable: true),
                    DatabaseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Error = table.Column<int>(type: "int", nullable: true),
                    EventSequence = table.Column<long>(type: "bigint", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: true),
                    HostName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IntegerData = table.Column<int>(type: "int", nullable: true),
                    IsSystem = table.Column<int>(type: "int", nullable: true),
                    LoginSid = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    NtdomainName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ObjectName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestId = table.Column<int>(type: "int", nullable: true),
                    RowCounts = table.Column<long>(type: "bigint", nullable: true),
                    ServerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionLoginName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionId = table.Column<long>(type: "bigint", nullable: true),
                    XactSequence = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraceMarches", x => x.RowNumber);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PassWord = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Maccess = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IntCustomerId = table.Column<int>(type: "int", nullable: true),
                    BitShowUser = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IntDisplayNo = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastChangeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    PriorityId = table.Column<int>(type: "int", nullable: true),
                    ApplicationId = table.Column<int>(type: "int", nullable: true),
                    DelegatedTo = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Visible = table.Column<bool>(type: "bit", nullable: true),
                    SeriousBug = table.Column<bool>(type: "bit", nullable: true),
                    SmallBug = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_Tasks_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "StatusId");
                    table.ForeignKey(
                        name: "FK_Tasks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Tasks_tblApplication_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "tblApplication",
                        principalColumn: "IntId");
                });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Priority");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "TasksOlds");

            migrationBuilder.DropTable(
                name: "TblAdmins");

            migrationBuilder.DropTable(
                name: "tblCustomer");

            migrationBuilder.DropTable(
                name: "tblPermission");

            migrationBuilder.DropTable(
                name: "TblUploadedFiles");

            migrationBuilder.DropTable(
                name: "TraceMarches");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "tblApplication");
        }
    }
}
