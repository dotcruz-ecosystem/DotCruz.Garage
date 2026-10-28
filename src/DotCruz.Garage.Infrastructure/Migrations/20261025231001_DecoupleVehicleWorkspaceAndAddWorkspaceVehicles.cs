using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotCruz.Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DecoupleVehicleWorkspaceAndAddWorkspaceVehicles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_vehicles_workspace_id_user_id",
                table: "vehicles");

            migrationBuilder.DropIndex(
                name: "ix_service_orders_workspace_id_user_id",
                table: "service_orders");

            migrationBuilder.DropIndex(
                name: "ix_maintenance_schedules_workspace_id_user_id",
                table: "maintenance_schedules");

            migrationBuilder.DropColumn(
                name: "workspace_id",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "workspace_id",
                table: "service_orders");

            migrationBuilder.DropColumn(
                name: "workspace_id",
                table: "maintenance_schedules");

            migrationBuilder.AddColumn<Guid>(
                name: "origin_workspace_id",
                table: "service_orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "origin_workspace_id",
                table: "maintenance_schedules",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "workspace_vehicles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workspace_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_workspace_vehicles", x => x.id);
                    table.ForeignKey(
                        name: "fk_workspace_vehicles_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_user_id",
                table: "vehicles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_orders_origin_workspace_id",
                table: "service_orders",
                column: "origin_workspace_id");

            migrationBuilder.CreateIndex(
                name: "ix_service_orders_user_id",
                table: "service_orders",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_schedules_origin_workspace_id",
                table: "maintenance_schedules",
                column: "origin_workspace_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_schedules_user_id",
                table: "maintenance_schedules",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_workspace_vehicles_vehicle_id",
                table: "workspace_vehicles",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_workspace_vehicles_workspace_id",
                table: "workspace_vehicles",
                column: "workspace_id");

            migrationBuilder.CreateIndex(
                name: "ix_workspace_vehicles_workspace_id_vehicle_id",
                table: "workspace_vehicles",
                columns: new[] { "workspace_id", "vehicle_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "workspace_vehicles");

            migrationBuilder.DropIndex(
                name: "ix_vehicles_user_id",
                table: "vehicles");

            migrationBuilder.DropIndex(
                name: "ix_service_orders_origin_workspace_id",
                table: "service_orders");

            migrationBuilder.DropIndex(
                name: "ix_service_orders_user_id",
                table: "service_orders");

            migrationBuilder.DropIndex(
                name: "ix_maintenance_schedules_origin_workspace_id",
                table: "maintenance_schedules");

            migrationBuilder.DropIndex(
                name: "ix_maintenance_schedules_user_id",
                table: "maintenance_schedules");

            migrationBuilder.DropColumn(
                name: "origin_workspace_id",
                table: "service_orders");

            migrationBuilder.DropColumn(
                name: "origin_workspace_id",
                table: "maintenance_schedules");

            migrationBuilder.AddColumn<Guid>(
                name: "workspace_id",
                table: "vehicles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "workspace_id",
                table: "service_orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "workspace_id",
                table: "maintenance_schedules",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_vehicles_workspace_id_user_id",
                table: "vehicles",
                columns: new[] { "workspace_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_service_orders_workspace_id_user_id",
                table: "service_orders",
                columns: new[] { "workspace_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_schedules_workspace_id_user_id",
                table: "maintenance_schedules",
                columns: new[] { "workspace_id", "user_id" });
        }
    }
}
