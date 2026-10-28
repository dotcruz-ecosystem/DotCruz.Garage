using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotCruz.Garage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMaintenanceCheckpointsAndStrictGenerationYears : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "maintenance_plan_template_checkpoints",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    template_id = table.Column<Guid>(type: "uuid", nullable: false),
                    odometer = table.Column<int>(type: "integer", nullable: false),
                    months = table.Column<int>(type: "integer", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenance_plan_template_checkpoints", x => x.id);
                    table.ForeignKey(
                        name: "fk_maintenance_plan_template_checkpoints_maintenance_plan_temp",
                        column: x => x.template_id,
                        principalTable: "maintenance_plan_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "maintenance_schedule_checkpoints",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    odometer = table.Column<int>(type: "integer", nullable: false),
                    months = table.Column<int>(type: "integer", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    completed_odometer = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maintenance_schedule_checkpoints", x => x.id);
                    table.ForeignKey(
                        name: "fk_maintenance_schedule_checkpoints_maintenance_schedules_sche",
                        column: x => x.schedule_id,
                        principalTable: "maintenance_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_plan_template_checkpoints_template_id",
                table: "maintenance_plan_template_checkpoints",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "ix_maintenance_schedule_checkpoints_schedule_id",
                table: "maintenance_schedule_checkpoints",
                column: "schedule_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "maintenance_plan_template_checkpoints");

            migrationBuilder.DropTable(
                name: "maintenance_schedule_checkpoints");
        }
    }
}
