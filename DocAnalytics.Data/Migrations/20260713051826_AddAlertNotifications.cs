using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocAnalytics.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "alert_notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    alert_rule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    observed_percent = table.Column<double>(type: "double precision", nullable: false),
                    threshold_percent = table.Column<double>(type: "double precision", nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false),
                    fired_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_alert_notifications", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_alert_notifications_tenant_site_read_fired",
                table: "alert_notifications",
                columns: new[] { "tenant_id", "site_id", "is_read", "fired_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alert_notifications");
        }
    }
}
