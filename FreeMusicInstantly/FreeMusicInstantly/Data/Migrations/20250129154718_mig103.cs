using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreeMusicInstantly.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig103 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Like_AspNetUsers_UserId1",
                table: "Like");

            migrationBuilder.DropForeignKey(
                name: "FK_Like_Songs_SongId",
                table: "Like");

            migrationBuilder.DropForeignKey(
                name: "FK_Play_AspNetUsers_UserId1",
                table: "Play");

            migrationBuilder.DropForeignKey(
                name: "FK_Play_Songs_SongId",
                table: "Play");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Play",
                table: "Play");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Like",
                table: "Like");

            migrationBuilder.RenameTable(
                name: "Play",
                newName: "Plays");

            migrationBuilder.RenameTable(
                name: "Like",
                newName: "Likes");

            migrationBuilder.RenameIndex(
                name: "IX_Play_UserId1",
                table: "Plays",
                newName: "IX_Plays_UserId1");

            migrationBuilder.RenameIndex(
                name: "IX_Play_SongId",
                table: "Plays",
                newName: "IX_Plays_SongId");

            migrationBuilder.RenameIndex(
                name: "IX_Like_UserId1",
                table: "Likes",
                newName: "IX_Likes_UserId1");

            migrationBuilder.RenameIndex(
                name: "IX_Like_SongId",
                table: "Likes",
                newName: "IX_Likes_SongId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Plays",
                table: "Plays",
                column: "PlayId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Likes",
                table: "Likes",
                column: "LikeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_AspNetUsers_UserId1",
                table: "Likes",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Songs_SongId",
                table: "Likes",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Plays_AspNetUsers_UserId1",
                table: "Plays",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Plays_Songs_SongId",
                table: "Plays",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_AspNetUsers_UserId1",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Songs_SongId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Plays_AspNetUsers_UserId1",
                table: "Plays");

            migrationBuilder.DropForeignKey(
                name: "FK_Plays_Songs_SongId",
                table: "Plays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Plays",
                table: "Plays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Likes",
                table: "Likes");

            migrationBuilder.RenameTable(
                name: "Plays",
                newName: "Play");

            migrationBuilder.RenameTable(
                name: "Likes",
                newName: "Like");

            migrationBuilder.RenameIndex(
                name: "IX_Plays_UserId1",
                table: "Play",
                newName: "IX_Play_UserId1");

            migrationBuilder.RenameIndex(
                name: "IX_Plays_SongId",
                table: "Play",
                newName: "IX_Play_SongId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_UserId1",
                table: "Like",
                newName: "IX_Like_UserId1");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_SongId",
                table: "Like",
                newName: "IX_Like_SongId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Play",
                table: "Play",
                column: "PlayId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Like",
                table: "Like",
                column: "LikeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Like_AspNetUsers_UserId1",
                table: "Like",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Like_Songs_SongId",
                table: "Like",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Play_AspNetUsers_UserId1",
                table: "Play",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Play_Songs_SongId",
                table: "Play",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
