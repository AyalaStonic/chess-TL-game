using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameId1",
                table: "Moves",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlayedAt",
                table: "Moves",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Games",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Moves_GameId1",
                table: "Moves",
                column: "GameId1");

            migrationBuilder.CreateIndex(
                name: "IX_Games_UserId",
                table: "Games",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_UserId1",
                table: "Games",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Users_UserId",
                table: "Games",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Users_UserId1",
                table: "Games",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Moves_Games_GameId1",
                table: "Moves",
                column: "GameId1",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Users_UserId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Users_UserId1",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Moves_Games_GameId1",
                table: "Moves");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Moves_GameId1",
                table: "Moves");

            migrationBuilder.DropIndex(
                name: "IX_Games_UserId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_UserId1",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "GameId1",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "PlayedAt",
                table: "Moves");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Games");
        }
    }
}
