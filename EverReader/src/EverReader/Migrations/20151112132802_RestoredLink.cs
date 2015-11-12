using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace EverReader.Migrations
{
    public partial class RestoredLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("TesterModel");
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
            migrationBuilder.CreateTable(
                name: "TesterModel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NameOfTest = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TesterModel", x => x.Id);
                });
        }
    }
}
