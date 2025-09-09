using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskList_Server.Migrations
{
    /// <inheritdoc />
    public partial class _29082025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TraceMarches",
                table: "TraceMarches");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TraceMarches");

            migrationBuilder.AlterColumn<int>(
                name: "RowNumber",
                table: "TraceMarches",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TraceMarches",
                table: "TraceMarches",
                column: "RowNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TraceMarches",
                table: "TraceMarches");

            migrationBuilder.AlterColumn<int>(
                name: "RowNumber",
                table: "TraceMarches",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TraceMarches",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TraceMarches",
                table: "TraceMarches",
                column: "Id");
        }
    }
}
