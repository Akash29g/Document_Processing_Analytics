using DocAnalytics.Data;
using DocAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Service.ErrorCatalog;

/// <summary>Default <see cref="IErrorCatalogService"/> implementation.</summary>
public sealed class ErrorCatalogService : IErrorCatalogService
{
    private readonly AppDbContext _db;

    public ErrorCatalogService(AppDbContext db) => _db = db;

    public async Task<List<ErrorCatalogDto>> GetAllAsync(CancellationToken ct = default)
        => await _db.ErrorCatalog
            .AsNoTracking()
            .OrderBy(e => e.ErrorCode)
            .Select(e => ToDto(e))
            .ToListAsync(ct);

    public async Task<ErrorCatalogDto?> CreateAsync(
        CreateErrorCatalogDto dto, CancellationToken ct = default)
    {
        var code = dto.ErrorCode.Trim().ToUpperInvariant();

        // Unique error code guard (DB has a unique index, but we surface a clean error)
        if (await _db.ErrorCatalog.AnyAsync(e => e.ErrorCode == code, ct))
            return null;   // caller maps to 409

        var now = DateTime.UtcNow;
        var entry = new Domain.Entities.ErrorCatalog
        {
            Id = Guid.NewGuid(),
            ErrorCode = code,
            Description = dto.Description.Trim(),
            RemediationMsg = dto.RemediationMsg?.Trim(),
            CreatedAt = now,
            UpdatedAt = now,
        };

        _db.ErrorCatalog.Add(entry);
        await _db.SaveChangesAsync(ct);
        return ToDto(entry);
    }

    public async Task<ErrorCatalogDto?> UpdateAsync(
        Guid id, UpdateErrorCatalogDto dto, CancellationToken ct = default)
    {
        // FindAsync bypasses query filters → correct for a global (non-tenant) table
        var entry = await _db.ErrorCatalog.FindAsync(new object[] { id }, ct);
        if (entry is null) return null;   // caller maps to 404

        entry.Description = dto.Description.Trim();
        entry.RemediationMsg = dto.RemediationMsg?.Trim();
        entry.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return ToDto(entry);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entry = await _db.ErrorCatalog.FindAsync(new object[] { id }, ct);
        if (entry is null) return false;
        _db.ErrorCatalog.Remove(entry);
        await _db.SaveChangesAsync(ct);
        return true;
    }


    // ── projection helper ────────────────────────────────────────────────────
    private static ErrorCatalogDto ToDto(Domain.Entities.ErrorCatalog e) => new()
    {
        Id = e.Id,
        ErrorCode = e.ErrorCode,
        Description = e.Description,
        RemediationMsg = e.RemediationMsg,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt,
    };
}
