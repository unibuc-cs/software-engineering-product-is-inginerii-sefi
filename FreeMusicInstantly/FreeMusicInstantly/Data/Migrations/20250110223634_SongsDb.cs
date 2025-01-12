﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreeMusicInstantly.Data.Migrations
{
    /// <inheritdoc />
    public partial class SongsDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SongFile",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SongFile",
                table: "Songs");
        }
    }
}
