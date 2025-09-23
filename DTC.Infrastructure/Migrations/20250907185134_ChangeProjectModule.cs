using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DTC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeProjectModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BeginAge",
                table: "Projects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndAge",
                table: "Projects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsOpenSource",
                table: "Projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Authors",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Authors",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondName",
                table: "Authors",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegUserId",
                table: "AuthorGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AuthorGroups_RegUserId",
                table: "AuthorGroups",
                column: "RegUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorGroups_AspNetUsers_RegUserId",
                table: "AuthorGroups",
                column: "RegUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorGroups_AspNetUsers_RegUserId",
                table: "AuthorGroups");

            migrationBuilder.DropIndex(
                name: "IX_AuthorGroups_RegUserId",
                table: "AuthorGroups");

            migrationBuilder.DropColumn(
                name: "BeginAge",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "EndAge",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "IsOpenSource",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "SecondName",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "RegUserId",
                table: "AuthorGroups");
        }
    }
}
