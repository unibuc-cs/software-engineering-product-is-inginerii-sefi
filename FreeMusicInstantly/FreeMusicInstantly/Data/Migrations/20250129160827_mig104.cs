using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreeMusicInstantly.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig104 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_AspNetUsers_UserId1",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Plays_AspNetUsers_UserId1",
                table: "Plays");

            migrationBuilder.DropIndex(
                name: "IX_Plays_UserId1",
                table: "Plays");

            migrationBuilder.DropIndex(
                name: "IX_Likes_UserId1",
                table: "Likes");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Plays");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Likes");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Plays",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Likes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Plays_UserId",
                table: "Plays",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId",
                table: "Likes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_AspNetUsers_UserId",
                table: "Likes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Plays_AspNetUsers_UserId",
                table: "Plays",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_AspNetUsers_UserId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Plays_AspNetUsers_UserId",
                table: "Plays");

            migrationBuilder.DropIndex(
                name: "IX_Plays_UserId",
                table: "Plays");

            migrationBuilder.DropIndex(
                name: "IX_Likes_UserId",
                table: "Likes");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Plays",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Plays",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Likes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Likes",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plays_UserId1",
                table: "Plays",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId1",
                table: "Likes",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_AspNetUsers_UserId1",
                table: "Likes",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Plays_AspNetUsers_UserId1",
                table: "Plays",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
