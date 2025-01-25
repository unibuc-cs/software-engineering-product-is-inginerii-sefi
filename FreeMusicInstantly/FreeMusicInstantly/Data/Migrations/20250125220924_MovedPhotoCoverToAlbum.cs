using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreeMusicInstantly.Data.Migrations
{
    /// <inheritdoc />
    public partial class MovedPhotoCoverToAlbum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo_Cover",
                table: "Songs");

            migrationBuilder.AddColumn<string>(
                name: "PhotoCover",
                table: "Playlists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoCover",
                table: "Albums",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoCover",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "PhotoCover",
                table: "Albums");

            migrationBuilder.AddColumn<string>(
                name: "Photo_Cover",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
