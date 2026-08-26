using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotCruz.Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleBlueprintsAndCatalogTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vehicle_blueprints",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    engine = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    version = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicle_blueprints", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "maintenance_plan_templates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_blueprint_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    category = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    interval_odometer = table.Column<int>(type: "integer", nullable: true),
                    interval_time_months = table.Column<int>(type: "integer", nullable: true),
                    items = table.Column<string>(type: "jsonb", nullable: false),
                    is_recurring = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenance_plan_templates", x => x.id);
                    table.ForeignKey(
                        name: "fk_maintenance_plan_templates_vehicle_blueprints_vehicle_bluep",
                        column: x => x.vehicle_blueprint_id,
                        principalTable: "vehicle_blueprints",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_plan_templates_vehicle_blueprint_id",
                table: "maintenance_plan_templates",
                column: "vehicle_blueprint_id");

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_blueprints_brand_model_year_engine",
                table: "vehicle_blueprints",
                columns: new[] { "brand", "model", "year", "engine" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "maintenance_plan_templates");

            migrationBuilder.DropTable(
                name: "vehicle_blueprints");
        }
    }
}
