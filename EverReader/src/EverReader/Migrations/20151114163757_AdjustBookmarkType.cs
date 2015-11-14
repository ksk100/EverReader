using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace EverReader.Migrations
{
    public partial class AdjustBookmarkType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Position", table: "Bookmark");
            migrationBuilder.AddColumn<decimal>(
                name: "PercentageRead",
                table: "Bookmark",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "PercentageRead", table: "Bookmark");
            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Bookmark",
                nullable: false,
                defaultValue: 0);
        }
    }
}
