using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotCruz.Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomWarningThresholds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "warning_threshold_days",
                table: "vehicle_documents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "warning_threshold_days",
                table: "traffic_fines",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "warning_threshold_days",
                table: "maintenance_schedules",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "warning_threshold_odometer",
                table: "maintenance_schedules",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_maintenance_schedules_vehicles_vehicle_id",
                table: "maintenance_schedules",
                column: "vehicle_id",
                principalTable: "vehicles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_traffic_fines_vehicles_vehicle_id",
                table: "traffic_fines",
                column: "vehicle_id",
                principalTable: "vehicles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_documents_vehicles_vehicle_id",
                table: "vehicle_documents",
                column: "vehicle_id",
                principalTable: "vehicles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_maintenance_schedules_vehicles_vehicle_id",
                table: "maintenance_schedules");

            migrationBuilder.DropForeignKey(
                name: "fk_traffic_fines_vehicles_vehicle_id",
                table: "traffic_fines");

            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_documents_vehicles_vehicle_id",
                table: "vehicle_documents");

            migrationBuilder.DropColumn(
                name: "warning_threshold_days",
                table: "vehicle_documents");

            migrationBuilder.DropColumn(
                name: "warning_threshold_days",
                table: "traffic_fines");

            migrationBuilder.DropColumn(
                name: "warning_threshold_days",
                table: "maintenance_schedules");

            migrationBuilder.DropColumn(
                name: "warning_threshold_odometer",
                table: "maintenance_schedules");
        }
    }
}
