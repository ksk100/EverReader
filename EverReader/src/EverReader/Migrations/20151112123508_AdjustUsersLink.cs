using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace EverReader.Migrations
{
    public partial class AdjustUsersLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_ApplicationUser_EFDbEvernoteCredentials_EvernoteCredentialsId", table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUser_EFDbEvernoteCredentials_EvernoteCredentialsId",
                table: "AspNetUsers",
                column: "EvernoteCredentialsId",
                principalTable: "EFDbEvernoteCredentials",
                principalColumn: "Id");
        }
    }
}
