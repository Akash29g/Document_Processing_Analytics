using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocAnalytics.Data.Migrations;

/// <inheritdoc />
public partial class AddTwoFactorAuth : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "two_factor_enabled",
            table: "users",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<string>(
            name: "two_factor_secret",
            table: "users",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "ip_address",
            table: "refresh_tokens",
            type: "character varying(64)",
            maxLength: 64,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "last_used_at",
            table: "refresh_tokens",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "user_agent",
            table: "refresh_tokens",
            type: "character varying(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.CreateTable(
            name: "two_factor_recovery_codes",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                code_hash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_two_factor_recovery_codes", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_two_factor_recovery_codes_user_id",
            table: "two_factor_recovery_codes",
            column: "user_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "two_factor_recovery_codes");

        migrationBuilder.DropColumn(
            name: "two_factor_enabled",
            table: "users");

        migrationBuilder.DropColumn(
            name: "two_factor_secret",
            table: "users");

        migrationBuilder.DropColumn(
            name: "ip_address",
            table: "refresh_tokens");

        migrationBuilder.DropColumn(
            name: "last_used_at",
            table: "refresh_tokens");

        migrationBuilder.DropColumn(
            name: "user_agent",
            table: "refresh_tokens");
    }
}
