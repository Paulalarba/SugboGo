using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SugboGo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTravelSpots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TravelSpots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Location = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false),
                    Category = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Region = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    IsPopular = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelSpots", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TravelSpots_Category",
                table: "TravelSpots",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_TravelSpots_Name",
                table: "TravelSpots",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TravelSpots_Region",
                table: "TravelSpots",
                column: "Region");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TravelSpots");
        }
    }
}
