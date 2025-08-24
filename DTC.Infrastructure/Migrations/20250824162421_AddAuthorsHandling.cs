using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DTC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorsHandling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorGroupsMember_AspNetUsers_AuthorId",
                table: "AuthorGroupsMember");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorGroupsMember_AuthorGroups_AuthorGroupId",
                table: "AuthorGroupsMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthorGroupsMember",
                table: "AuthorGroupsMember");

            migrationBuilder.DropIndex(
                name: "IX_AuthorGroupsMember_AuthorGroupId",
                table: "AuthorGroupsMember");

            migrationBuilder.DropIndex(
                name: "IX_AuthorGroupsMember_AuthorId",
                table: "AuthorGroupsMember");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AuthorGroupsMember");

            migrationBuilder.DropColumn(
                name: "AuthorGroupId",
                table: "AuthorGroupsMember");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "AuthorGroupsMember");

            migrationBuilder.DropColumn(
                name: "RegUser_ID",
                table: "AuthorGroupsMember");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthorGroupsMember",
                table: "AuthorGroupsMember",
                columns: new[] { "Author_ID", "AuthorGroup_ID" });

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    RegDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Authors_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorGroupsMember_AuthorGroup_ID",
                table: "AuthorGroupsMember",
                column: "AuthorGroup_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_UserId",
                table: "Authors",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorGroupsMember_AuthorGroups_AuthorGroup_ID",
                table: "AuthorGroupsMember",
                column: "AuthorGroup_ID",
                principalTable: "AuthorGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorGroupsMember_Authors_Author_ID",
                table: "AuthorGroupsMember",
                column: "Author_ID",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorGroupsMember_AuthorGroups_AuthorGroup_ID",
                table: "AuthorGroupsMember");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorGroupsMember_Authors_Author_ID",
                table: "AuthorGroupsMember");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthorGroupsMember",
                table: "AuthorGroupsMember");

            migrationBuilder.DropIndex(
                name: "IX_AuthorGroupsMember_AuthorGroup_ID",
                table: "AuthorGroupsMember");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AuthorGroupsMember",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "AuthorGroupId",
                table: "AuthorGroupsMember",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "AuthorGroupsMember",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RegUser_ID",
                table: "AuthorGroupsMember",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthorGroupsMember",
                table: "AuthorGroupsMember",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorGroupsMember_AuthorGroupId",
                table: "AuthorGroupsMember",
                column: "AuthorGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorGroupsMember_AuthorId",
                table: "AuthorGroupsMember",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorGroupsMember_AspNetUsers_AuthorId",
                table: "AuthorGroupsMember",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorGroupsMember_AuthorGroups_AuthorGroupId",
                table: "AuthorGroupsMember",
                column: "AuthorGroupId",
                principalTable: "AuthorGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
