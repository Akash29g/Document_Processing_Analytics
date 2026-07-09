using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocAnalytics.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "invoice_headers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_number = table.Column<string>(type: "text", nullable: true),
                    invoice_date = table.Column<string>(type: "text", nullable: true),
                    seller = table.Column<string>(type: "text", nullable: true),
                    buyer = table.Column<string>(type: "text", nullable: true),
                    currency = table.Column<string>(type: "text", nullable: true),
                    subtotal = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    discount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    tax = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    shipping = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    total = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    extracted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoice_headers", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoice_headers_files_file_id",
                        column: x => x.file_id,
                        principalTable: "files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_invoice_headers_file_id",
                table: "invoice_headers",
                column: "file_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invoice_headers");
        }
    }
}
