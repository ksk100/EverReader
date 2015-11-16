using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace EverReader.Migrations
{
    public partial class AdjustmentsToBookmarkModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NoteTitle",
                table: "Bookmark",
                nullable: true);
            migrationBuilder.AddColumn<DateTime>(
                name: "NoteUpdated",
                table: "Bookmark",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "NoteTitle", table: "Bookmark");
            migrationBuilder.DropColumn(name: "NoteUpdated", table: "Bookmark");
        }
    }
}
