using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace EverReader.Migrations
{
    public partial class ModelsForBookmarks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookmark",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NoteGuid = table.Column<string>(nullable: true),
                    Position = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookmark", x => x.Id);
                });
            migrationBuilder.AlterColumn<int>(
                name: "EvernoteCredentialsId",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUser_EFDbEvernoteCredentials_EvernoteCredentialsId",
                table: "AspNetUsers",
                column: "EvernoteCredentialsId",
                principalTable: "EFDbEvernoteCredentials",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_ApplicationUser_EFDbEvernoteCredentials_EvernoteCredentialsId", table: "AspNetUsers");
            migrationBuilder.DropTable("Bookmark");
            migrationBuilder.AlterColumn<int>(
                name: "EvernoteCredentialsId",
                table: "AspNetUsers",
                nullable: false);
        }
    }
}
