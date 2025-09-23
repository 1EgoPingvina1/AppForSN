using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeProjectModule_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegUser_ID",
                table: "AuthorGroups");

            migrationBuilder.AlterColumn<string>(
                name: "Photo",
                table: "AuthorGroups",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "AuthorGroups",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Photo",
                table: "AuthorGroups",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "AuthorGroups",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegUser_ID",
                table: "AuthorGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
