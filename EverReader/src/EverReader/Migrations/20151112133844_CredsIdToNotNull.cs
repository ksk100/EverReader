using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace EverReader.Migrations
{
    public partial class CredsIdToNotNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EvernoteCredentialsId",
                table: "AspNetUsers",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EvernoteCredentialsId",
                table: "AspNetUsers",
                nullable: true);
        }
    }
}
