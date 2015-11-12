using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace EverReader.Migrations
{
    public partial class RemoveAuthorisedUntiField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "EvernoteAuthorisedUntilDate", table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EvernoteAuthorisedUntilDate",
                table: "AspNetUsers",
                nullable: true);
        }
    }
}
