using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SugboGo.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConnectDatabaseArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Interests",
                table: "TravelPreferences",
                type: "text",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]");

            migrationBuilder.AddColumn<int>(
                name: "TravelSpotId",
                table: "SavedGems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TravelSpotId",
                table: "DestinationPosts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TravelSpotId",
                table: "Bookings",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedGems_TravelSpotId",
                table: "SavedGems",
                column: "TravelSpotId");

            migrationBuilder.CreateIndex(
                name: "IX_PostComment_UserId",
                table: "PostComment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DestinationPosts_TravelSpotId",
                table: "DestinationPosts",
                column: "TravelSpotId");

            migrationBuilder.CreateIndex(
                name: "IX_DestinationPosts_UserId",
                table: "DestinationPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TravelSpotId",
                table: "Bookings",
                column: "TravelSpotId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_TravelSpots_TravelSpotId",
                table: "Bookings",
                column: "TravelSpotId",
                principalTable: "TravelSpots",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DestinationPosts_TravelSpots_TravelSpotId",
                table: "DestinationPosts",
                column: "TravelSpotId",
                principalTable: "TravelSpots",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_DestinationPosts_Users_UserId",
                table: "DestinationPosts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostComment_Users_UserId",
                table: "PostComment",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedGems_TravelSpots_TravelSpotId",
                table: "SavedGems",
                column: "TravelSpotId",
                principalTable: "TravelSpots",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedGems_Users_UserId",
                table: "SavedGems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TravelPreferences_Users_UserId",
                table: "TravelPreferences",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_TravelSpots_TravelSpotId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_DestinationPosts_TravelSpots_TravelSpotId",
                table: "DestinationPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_DestinationPosts_Users_UserId",
                table: "DestinationPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_PostComment_Users_UserId",
                table: "PostComment");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedGems_TravelSpots_TravelSpotId",
                table: "SavedGems");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedGems_Users_UserId",
                table: "SavedGems");

            migrationBuilder.DropForeignKey(
                name: "FK_TravelPreferences_Users_UserId",
                table: "TravelPreferences");

            migrationBuilder.DropIndex(
                name: "IX_SavedGems_TravelSpotId",
                table: "SavedGems");

            migrationBuilder.DropIndex(
                name: "IX_PostComment_UserId",
                table: "PostComment");

            migrationBuilder.DropIndex(
                name: "IX_DestinationPosts_TravelSpotId",
                table: "DestinationPosts");

            migrationBuilder.DropIndex(
                name: "IX_DestinationPosts_UserId",
                table: "DestinationPosts");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TravelSpotId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TravelSpotId",
                table: "SavedGems");

            migrationBuilder.DropColumn(
                name: "TravelSpotId",
                table: "DestinationPosts");

            migrationBuilder.DropColumn(
                name: "TravelSpotId",
                table: "Bookings");

            migrationBuilder.AlterColumn<List<string>>(
                name: "Interests",
                table: "TravelPreferences",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
