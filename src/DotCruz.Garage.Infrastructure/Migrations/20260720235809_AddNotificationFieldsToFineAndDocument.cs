using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotCruz.Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationFieldsToFineAndDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_notified_at",
                table: "vehicle_documents",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_notified_status",
                table: "vehicle_documents",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_notified_at",
                table: "traffic_fines",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_notified_at",
                table: "vehicle_documents");

            migrationBuilder.DropColumn(
                name: "last_notified_status",
                table: "vehicle_documents");

            migrationBuilder.DropColumn(
                name: "last_notified_at",
                table: "traffic_fines");
        }
    }
}
