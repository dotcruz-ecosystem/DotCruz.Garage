using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotCruz.Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDriversAndNICFineProtection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "driver_id",
                table: "traffic_fines",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "indicated_at",
                table: "traffic_fines",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "indication_deadline",
                table: "traffic_fines",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "indication_protocol",
                table: "traffic_fines",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "issuing_authority",
                table: "traffic_fines",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "points",
                table: "traffic_fines",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "drivers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    cnh = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cnh_category = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    cnh_expiration_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_drivers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_driver_assignments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    driver_id = table.Column<Guid>(type: "uuid", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicle_driver_assignments", x => x.id);
                    table.ForeignKey(
                        name: "fk_vehicle_driver_assignments_drivers_driver_id",
                        column: x => x.driver_id,
                        principalTable: "drivers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_vehicle_driver_assignments_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_traffic_fines_driver_id",
                table: "traffic_fines",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "ix_traffic_fines_indication_deadline",
                table: "traffic_fines",
                column: "indication_deadline");

            migrationBuilder.CreateIndex(
                name: "ix_drivers_tenant_id_cnh",
                table: "drivers",
                columns: new[] { "tenant_id", "cnh" });

            migrationBuilder.CreateIndex(
                name: "ix_drivers_tenant_id_cpf",
                table: "drivers",
                columns: new[] { "tenant_id", "cpf" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_drivers_tenant_id_user_id",
                table: "drivers",
                columns: new[] { "tenant_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_driver_assignments_driver_id",
                table: "vehicle_driver_assignments",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_driver_assignments_tenant_id_vehicle_id_ended_at",
                table: "vehicle_driver_assignments",
                columns: new[] { "tenant_id", "vehicle_id", "ended_at" });

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_driver_assignments_vehicle_id",
                table: "vehicle_driver_assignments",
                column: "vehicle_id");

            migrationBuilder.AddForeignKey(
                name: "fk_traffic_fines_drivers_driver_id",
                table: "traffic_fines",
                column: "driver_id",
                principalTable: "drivers",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_traffic_fines_drivers_driver_id",
                table: "traffic_fines");

            migrationBuilder.DropTable(
                name: "vehicle_driver_assignments");

            migrationBuilder.DropTable(
                name: "drivers");

            migrationBuilder.DropIndex(
                name: "ix_traffic_fines_driver_id",
                table: "traffic_fines");

            migrationBuilder.DropIndex(
                name: "ix_traffic_fines_indication_deadline",
                table: "traffic_fines");

            migrationBuilder.DropColumn(
                name: "driver_id",
                table: "traffic_fines");

            migrationBuilder.DropColumn(
                name: "indicated_at",
                table: "traffic_fines");

            migrationBuilder.DropColumn(
                name: "indication_deadline",
                table: "traffic_fines");

            migrationBuilder.DropColumn(
                name: "indication_protocol",
                table: "traffic_fines");

            migrationBuilder.DropColumn(
                name: "issuing_authority",
                table: "traffic_fines");

            migrationBuilder.DropColumn(
                name: "points",
                table: "traffic_fines");
        }
    }
}
