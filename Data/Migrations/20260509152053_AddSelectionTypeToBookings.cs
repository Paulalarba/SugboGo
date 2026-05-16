using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SugboGo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSelectionTypeToBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SelectionType",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectionType",
                table: "Bookings");
        }
    }
}
