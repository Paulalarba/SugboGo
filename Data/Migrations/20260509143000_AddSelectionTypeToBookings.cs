using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SugboGo.Data.Migrations
{
    [Migration("20260509143000_AddSelectionTypeToBookings")]
    public partial class AddSelectionTypeToBookings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SelectionType",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "UserSelected");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectionType",
                table: "Bookings");
        }
    }
}
