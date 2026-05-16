using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SugboGo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdventureLevelAndPriceToTravelSpot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdventureLevel",
                table: "TravelSpots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "TravelSpots",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdventureLevel",
                table: "TravelSpots");

            migrationBuilder.DropColumn(
                name: "BasePrice",
                table: "TravelSpots");
        }
    }
}
