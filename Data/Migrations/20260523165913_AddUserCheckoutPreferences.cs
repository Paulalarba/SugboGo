using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SugboGo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCheckoutPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCheckoutPreferences",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    LastName = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    AddressLine1 = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    City = table.Column<string>(type: "character varying(90)", maxLength: 90, nullable: false),
                    StateProvince = table.Column<string>(type: "character varying(90)", maxLength: 90, nullable: false),
                    PostalCode = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    CardholderName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CardLast4 = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    CardExpiry = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCheckoutPreferences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCheckoutPreferences_UserId",
                table: "UserCheckoutPreferences",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCheckoutPreferences");
        }
    }
}
