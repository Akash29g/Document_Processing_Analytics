using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocAnalytics.Data.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "activity_log",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                site_id = table.Column<Guid>(type: "uuid", nullable: false),
                event_type = table.Column<string>(type: "text", nullable: false),
                entity_type = table.Column<string>(type: "text", nullable: false),
                entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                entity_name = table.Column<string>(type: "text", nullable: true),
                old_state = table.Column<string>(type: "text", nullable: true),
                new_state = table.Column<string>(type: "text", nullable: true),
                triggered_by = table.Column<string>(type: "text", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_activity_log", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "document_types",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                type_name = table.Column<string>(type: "text", nullable: false),
                category = table.Column<string>(type: "text", nullable: false),
                description = table.Column<string>(type: "text", nullable: true),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_document_types", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "error_catalog",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                error_code = table.Column<string>(type: "text", nullable: false),
                description = table.Column<string>(type: "text", nullable: false),
                remediation_msg = table.Column<string>(type: "text", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_error_catalog", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "item_categories",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                category_code = table.Column<string>(type: "text", nullable: false),
                category_name = table.Column<string>(type: "text", nullable: false),
                description = table.Column<string>(type: "text", nullable: true),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_item_categories", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "tenants",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_tenants", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "transactions",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                site_id = table.Column<Guid>(type: "uuid", nullable: false),
                state = table.Column<string>(type: "text", nullable: false),
                source_system = table.Column<string>(type: "text", nullable: false),
                total_files = table.Column<int>(type: "integer", nullable: false),
                uploaded_count = table.Column<int>(type: "integer", nullable: false),
                processing_count = table.Column<int>(type: "integer", nullable: false),
                failed_count = table.Column<int>(type: "integer", nullable: false),
                completed_count = table.Column<int>(type: "integer", nullable: false),
                submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                last_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_transactions", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "sites",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                location = table.Column<string>(type: "text", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_sites", x => x.id);
                table.ForeignKey(
                    name: "fk_sites_tenants_tenant_id",
                    column: x => x.tenant_id,
                    principalTable: "tenants",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "users",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                email = table.Column<string>(type: "text", nullable: false),
                password_hash = table.Column<string>(type: "text", nullable: false),
                role = table.Column<string>(type: "text", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users", x => x.id);
                table.ForeignKey(
                    name: "fk_users_tenants_tenant_id",
                    column: x => x.tenant_id,
                    principalTable: "tenants",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "files",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                site_id = table.Column<Guid>(type: "uuid", nullable: false),
                transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                document_type_id = table.Column<Guid>(type: "uuid", nullable: true),
                file_name = table.Column<string>(type: "text", nullable: false),
                file_type = table.Column<string>(type: "text", nullable: false),
                status = table.Column<string>(type: "text", nullable: false),
                current_step = table.Column<string>(type: "text", nullable: false),
                file_size_bytes = table.Column<long>(type: "bigint", nullable: true),
                extraction_status = table.Column<string>(type: "text", nullable: true),
                extraction_confidence = table.Column<decimal>(type: "numeric(4,3)", precision: 4, scale: 3, nullable: true),
                last_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_files", x => x.id);
                table.ForeignKey(
                    name: "fk_files_document_types_document_type_id",
                    column: x => x.document_type_id,
                    principalTable: "document_types",
                    principalColumn: "id");
                table.ForeignKey(
                    name: "fk_files_transactions_transaction_id",
                    column: x => x.transaction_id,
                    principalTable: "transactions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "user_site_access",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                site_id = table.Column<Guid>(type: "uuid", nullable: false),
                granted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_site_access", x => x.id);
                table.ForeignKey(
                    name: "fk_user_site_access_sites_site_id",
                    column: x => x.site_id,
                    principalTable: "sites",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_user_site_access_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "file_step_history",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                file_id = table.Column<Guid>(type: "uuid", nullable: false),
                document_type_id = table.Column<Guid>(type: "uuid", nullable: true),
                step_name = table.Column<string>(type: "text", nullable: false),
                status = table.Column<string>(type: "text", nullable: false),
                started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                error_code = table.Column<string>(type: "text", nullable: true),
                error_message = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_file_step_history", x => x.id);
                table.ForeignKey(
                    name: "fk_file_step_history_document_types_document_type_id",
                    column: x => x.document_type_id,
                    principalTable: "document_types",
                    principalColumn: "id");
                table.ForeignKey(
                    name: "fk_file_step_history_files_file_id",
                    column: x => x.file_id,
                    principalTable: "files",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "invoice_line_items",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                file_id = table.Column<Guid>(type: "uuid", nullable: false),
                tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                site_id = table.Column<Guid>(type: "uuid", nullable: false),
                item_category_id = table.Column<Guid>(type: "uuid", nullable: true),
                line_number = table.Column<int>(type: "integer", nullable: false),
                description = table.Column<string>(type: "text", nullable: false),
                quantity = table.Column<decimal>(type: "numeric(12,3)", precision: 12, scale: 3, nullable: true),
                unit_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                line_total = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                confidence = table.Column<decimal>(type: "numeric(4,3)", precision: 4, scale: 3, nullable: true),
                is_valid = table.Column<bool>(type: "boolean", nullable: false),
                extracted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_invoice_line_items", x => x.id);
                table.ForeignKey(
                    name: "fk_invoice_line_items_files_file_id",
                    column: x => x.file_id,
                    principalTable: "files",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_invoice_line_items_item_categories_item_category_id",
                    column: x => x.item_category_id,
                    principalTable: "item_categories",
                    principalColumn: "id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateIndex(
            name: "ix_activity_log_tenant_id_site_id_created_at",
            table: "activity_log",
            columns: new[] { "tenant_id", "site_id", "created_at" });

        migrationBuilder.CreateIndex(
            name: "ix_document_types_type_name",
            table: "document_types",
            column: "type_name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_error_catalog_error_code",
            table: "error_catalog",
            column: "error_code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_file_step_history_document_type_id",
            table: "file_step_history",
            column: "document_type_id");

        migrationBuilder.CreateIndex(
            name: "ix_file_step_history_file_id",
            table: "file_step_history",
            column: "file_id");

        migrationBuilder.CreateIndex(
            name: "ix_file_step_history_step_name_status",
            table: "file_step_history",
            columns: new[] { "step_name", "status" });

        migrationBuilder.CreateIndex(
            name: "ix_files_document_type_id",
            table: "files",
            column: "document_type_id");

        migrationBuilder.CreateIndex(
            name: "ix_files_tenant_id_site_id_document_type_id",
            table: "files",
            columns: new[] { "tenant_id", "site_id", "document_type_id" });

        migrationBuilder.CreateIndex(
            name: "ix_files_tenant_id_site_id_status_last_updated_at",
            table: "files",
            columns: new[] { "tenant_id", "site_id", "status", "last_updated_at" });

        migrationBuilder.CreateIndex(
            name: "ix_files_transaction_id",
            table: "files",
            column: "transaction_id");

        migrationBuilder.CreateIndex(
            name: "ix_invoice_line_items_file_id",
            table: "invoice_line_items",
            column: "file_id");

        migrationBuilder.CreateIndex(
            name: "ix_invoice_line_items_item_category_id",
            table: "invoice_line_items",
            column: "item_category_id");

        migrationBuilder.CreateIndex(
            name: "ix_invoice_line_items_tenant_id_site_id_item_category_id",
            table: "invoice_line_items",
            columns: new[] { "tenant_id", "site_id", "item_category_id" });

        migrationBuilder.CreateIndex(
            name: "ix_item_categories_category_code",
            table: "item_categories",
            column: "category_code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_sites_tenant_id",
            table: "sites",
            column: "tenant_id");

        migrationBuilder.CreateIndex(
            name: "ix_transactions_tenant_id_site_id_last_updated_at",
            table: "transactions",
            columns: new[] { "tenant_id", "site_id", "last_updated_at" });

        migrationBuilder.CreateIndex(
            name: "ix_transactions_tenant_id_site_id_state",
            table: "transactions",
            columns: new[] { "tenant_id", "site_id", "state" });

        migrationBuilder.CreateIndex(
            name: "ix_user_site_access_site_id",
            table: "user_site_access",
            column: "site_id");

        migrationBuilder.CreateIndex(
            name: "ix_user_site_access_user_id_site_id",
            table: "user_site_access",
            columns: new[] { "user_id", "site_id" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_email",
            table: "users",
            column: "email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_tenant_id",
            table: "users",
            column: "tenant_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "activity_log");

        migrationBuilder.DropTable(
            name: "error_catalog");

        migrationBuilder.DropTable(
            name: "file_step_history");

        migrationBuilder.DropTable(
            name: "invoice_line_items");

        migrationBuilder.DropTable(
            name: "user_site_access");

        migrationBuilder.DropTable(
            name: "files");

        migrationBuilder.DropTable(
            name: "item_categories");

        migrationBuilder.DropTable(
            name: "sites");

        migrationBuilder.DropTable(
            name: "users");

        migrationBuilder.DropTable(
            name: "document_types");

        migrationBuilder.DropTable(
            name: "transactions");

        migrationBuilder.DropTable(
            name: "tenants");
    }
}
