using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocAnalytics.Data.Migrations;

/// <inheritdoc />
public partial class AddFileStorageKey : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "storage_key",
            table: "files",
            type: "text",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "storage_key",
            table: "files");
    }
}
