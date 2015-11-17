using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace EverReader.Migrations
{
    public partial class BookmarkNoteLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NoteLength",
                table: "Bookmark",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "NoteLength", table: "Bookmark");
        }
    }
}
