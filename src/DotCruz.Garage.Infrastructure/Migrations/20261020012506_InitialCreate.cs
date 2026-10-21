using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotCruz.Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "maintenance_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workspace_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    maintenance_schedule_id = table.Column<Guid>(type: "uuid", nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    performed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    odometer_at_service = table.Column<int>(type: "integer", nullable: false),
                    cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    receipt_url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenance_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workspace_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    brand = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    plate = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    current_odometer = table.Column<int>(type: "integer", nullable: false),
                    daily_mileage_average = table.Column<double>(type: "double precision", nullable: false),
                    odometer_updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "maintenance_schedules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workspace_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    category = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    interval_odometer = table.Column<int>(type: "integer", nullable: true),
                    interval_time_months = table.Column<int>(type: "integer", nullable: true),
                    last_performed_odometer = table.Column<int>(type: "integer", nullable: true),
                    last_performed_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    next_due_odometer = table.Column<int>(type: "integer", nullable: true),
                    next_due_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_notified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_notified_status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    warning_threshold_odometer = table.Column<int>(type: "integer", nullable: true),
                    warning_threshold_days = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenance_schedules", x => x.id);
                    table.ForeignKey(
                        name: "fk_maintenance_schedules_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "odometer_histories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    odometer_value = table.Column<int>(type: "integer", nullable: false),
                    recorded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_odometer_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_odometer_histories_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_logs_vehicle_id",
                table: "maintenance_logs",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_logs_workspace_id_user_id",
                table: "maintenance_logs",
                columns: new[] { "workspace_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_schedules_vehicle_id",
                table: "maintenance_schedules",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_schedules_workspace_id_user_id",
                table: "maintenance_schedules",
                columns: new[] { "workspace_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_odometer_histories_vehicle_id",
                table: "odometer_histories",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_workspace_id_user_id",
                table: "vehicles",
                columns: new[] { "workspace_id", "user_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "maintenance_logs");

            migrationBuilder.DropTable(
                name: "maintenance_schedules");

            migrationBuilder.DropTable(
                name: "odometer_histories");

            migrationBuilder.DropTable(
                name: "vehicles");
        }
    }
}
