using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocAnalytics.Data.Migrations;

/// <inheritdoc />
public partial class AddAlertRules : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "alert_rules",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                site_id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                threshold_percent = table.Column<double>(type: "double precision", nullable: false),
                window_minutes = table.Column<int>(type: "integer", nullable: false),
                email = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                is_enabled = table.Column<bool>(type: "boolean", nullable: false),
                cooldown_minutes = table.Column<int>(type: "integer", nullable: false),
                last_triggered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_alert_rules", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_alert_rules_tenant_id_site_id",
            table: "alert_rules",
            columns: new[] { "tenant_id", "site_id" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "alert_rules");
    }
}
