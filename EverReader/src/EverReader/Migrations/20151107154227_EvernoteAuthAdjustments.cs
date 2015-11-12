using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace EverReader.Migrations
{
    public partial class EvernoteAuthAdjustments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EFDbEvernoteCredentials",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuthToken = table.Column<string>(nullable: true),
                    Expires = table.Column<DateTime>(nullable: false),
                    NotebookUrl = table.Column<string>(nullable: true),
                    Shard = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    WebApiUrlPrefix = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EFDbEvernoteCredentials", x => x.Id);
                });
            migrationBuilder.AddColumn<DateTime>(
                name: "EvernoteAuthorisedUntilDate",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<int>(
                name: "EvernoteCredentialsId",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddColumn<bool>(
                name: "HasAuthorisedEvernote",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
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
            migrationBuilder.DropColumn(name: "EvernoteAuthorisedUntilDate", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "EvernoteCredentialsId", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "HasAuthorisedEvernote", table: "AspNetUsers");
            migrationBuilder.DropTable("EFDbEvernoteCredentials");
        }
    }
}
