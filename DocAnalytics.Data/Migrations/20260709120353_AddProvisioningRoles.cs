using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocAnalytics.Data.Migrations;

/// <inheritdoc />
public partial class AddProvisioningRoles : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<Guid>(
            name: "tenant_id",
            table: "users",
            type: "uuid",
            nullable: true,
            oldClrType: typeof(Guid),
            oldType: "uuid");

        migrationBuilder.AddColumn<Guid>(
            name: "created_by",
            table: "users",
            type: "uuid",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "must_change_password",
            table: "users",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<string>(
            name: "org_domain",
            table: "tenants",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddCheckConstraint(
            name: "ck_users_role",
            table: "users",
            sql: "role IN ('Developer','Admin','Viewer')");

        migrationBuilder.CreateIndex(
            name: "ix_tenants_org_domain",
            table: "tenants",
            column: "org_domain",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropCheckConstraint(
            name: "ck_users_role",
            table: "users");

        migrationBuilder.DropIndex(
            name: "ix_tenants_org_domain",
            table: "tenants");

        migrationBuilder.DropColumn(
            name: "created_by",
            table: "users");

        migrationBuilder.DropColumn(
            name: "must_change_password",
            table: "users");

        migrationBuilder.DropColumn(
            name: "org_domain",
            table: "tenants");

        migrationBuilder.AlterColumn<Guid>(
            name: "tenant_id",
            table: "users",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldNullable: true);
    }
}
