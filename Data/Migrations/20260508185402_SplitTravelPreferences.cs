using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SugboGo.Data.Migrations
{
    /// <inheritdoc />
    public partial class SplitTravelPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Interests",
                table: "TravelPreferences",
                newName: "PlaceInterests");

            migrationBuilder.AddColumn<string>(
                name: "ActivityInterests",
                table: "TravelPreferences",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityInterests",
                table: "TravelPreferences");

            migrationBuilder.RenameColumn(
                name: "PlaceInterests",
                table: "TravelPreferences",
                newName: "Interests");
        }
    }
}
