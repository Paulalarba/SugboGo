using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SugboGo.Data.Migrations
{
    /// <inheritdoc />
    public partial class RepairBookingsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE "Bookings"
                    ADD COLUMN IF NOT EXISTS "TravelSpotId" integer,
                    ADD COLUMN IF NOT EXISTS "ImageUrl" text,
                    ADD COLUMN IF NOT EXISTS "Location" text NOT NULL DEFAULT 'Cebu, Philippines',
                    ADD COLUMN IF NOT EXISTS "TravelerType" text NOT NULL DEFAULT 'Solo',
                    ADD COLUMN IF NOT EXISTS "TravelerCount" integer NOT NULL DEFAULT 1,
                    ADD COLUMN IF NOT EXISTS "SelectedActivitiesJson" text NOT NULL DEFAULT '[]',
                    ADD COLUMN IF NOT EXISTS "SelectedAccommodationJson" text NOT NULL DEFAULT '{}',
                    ADD COLUMN IF NOT EXISTS "SelectedTransportationJson" text NOT NULL DEFAULT '{}',
                    ADD COLUMN IF NOT EXISTS "BasePrice" numeric(18,2) NOT NULL DEFAULT 0,
                    ADD COLUMN IF NOT EXISTS "AddOnsPrice" numeric(18,2) NOT NULL DEFAULT 0,
                    ADD COLUMN IF NOT EXISTS "TaxesAndFees" numeric(18,2) NOT NULL DEFAULT 0,
                    ADD COLUMN IF NOT EXISTS "TotalPrice" numeric(18,2) NOT NULL DEFAULT 0,
                    ADD COLUMN IF NOT EXISTS "TravelerNotes" text,
                    ADD COLUMN IF NOT EXISTS "Status" text NOT NULL DEFAULT 'Pending',
                    ADD COLUMN IF NOT EXISTS "PaymentMethod" text,
                    ADD COLUMN IF NOT EXISTS "QrCode" text NOT NULL DEFAULT upper(substr(replace(gen_random_uuid()::text, '-', ''), 1, 8)),
                    ADD COLUMN IF NOT EXISTS "CreatedAt" timestamp with time zone NOT NULL DEFAULT now();
                """);

            migrationBuilder.Sql(
                """
                CREATE INDEX IF NOT EXISTS "IX_Bookings_UserId"
                    ON "Bookings" ("UserId");
                """);

            migrationBuilder.Sql(
                """
                CREATE INDEX IF NOT EXISTS "IX_Bookings_TravelSpotId"
                    ON "Bookings" ("TravelSpotId");
                """);

            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1
                        FROM pg_constraint
                        WHERE conname = 'FK_Bookings_Users_UserId'
                    ) THEN
                        ALTER TABLE "Bookings"
                            ADD CONSTRAINT "FK_Bookings_Users_UserId"
                            FOREIGN KEY ("UserId")
                            REFERENCES "Users" ("Id")
                            ON DELETE CASCADE;
                    END IF;

                    IF NOT EXISTS (
                        SELECT 1
                        FROM pg_constraint
                        WHERE conname = 'FK_Bookings_TravelSpots_TravelSpotId'
                    ) THEN
                        ALTER TABLE "Bookings"
                            ADD CONSTRAINT "FK_Bookings_TravelSpots_TravelSpotId"
                            FOREIGN KEY ("TravelSpotId")
                            REFERENCES "TravelSpots" ("Id")
                            ON DELETE SET NULL;
                    END IF;
                END $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE "Bookings"
                    DROP COLUMN IF EXISTS "TravelSpotId",
                    DROP COLUMN IF EXISTS "ImageUrl",
                    DROP COLUMN IF EXISTS "Location",
                    DROP COLUMN IF EXISTS "TravelerType",
                    DROP COLUMN IF EXISTS "TravelerCount",
                    DROP COLUMN IF EXISTS "SelectedActivitiesJson",
                    DROP COLUMN IF EXISTS "SelectedAccommodationJson",
                    DROP COLUMN IF EXISTS "SelectedTransportationJson",
                    DROP COLUMN IF EXISTS "BasePrice",
                    DROP COLUMN IF EXISTS "AddOnsPrice",
                    DROP COLUMN IF EXISTS "TaxesAndFees",
                    DROP COLUMN IF EXISTS "TotalPrice",
                    DROP COLUMN IF EXISTS "TravelerNotes",
                    DROP COLUMN IF EXISTS "Status",
                    DROP COLUMN IF EXISTS "PaymentMethod",
                    DROP COLUMN IF EXISTS "QrCode",
                    DROP COLUMN IF EXISTS "CreatedAt";
                """);
        }
    }
}
