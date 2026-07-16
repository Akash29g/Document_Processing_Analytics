using DocAnalytics.Domain.Entities;
using DocAnalytics.Tests.Support;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Data.Tests.Persistence;

public class NonScopedEntityIsolationTests
{
    // FileStepHistory + InvoiceHeader are NOT ITenantScoped (no global filter).
    // They are only safe when reached via an already-filtered File.
    // This proves: filter the File first => the child rows are unreachable cross-tenant.

    [Fact]
    public async Task Step_history_is_unreachable_when_parent_file_is_filtered_out()
    {
        var tenantB = Guid.NewGuid();
        var siteB = Guid.NewGuid();

        // context is tenant A
        using var db = TestDb.Create(new FakeCurrentUser
        {
            TenantId = Guid.NewGuid(),
            SiteId = Guid.NewGuid(),
        });

        var fileB = new FileRecord
        {
            Id = Guid.NewGuid(),
            TenantId = tenantB,
            SiteId = siteB,
            FileName = "tenant-b.pdf",   // required
            FileType = "pdf",            // required
            Status = "Failed",         // required
            CurrentStep = "Validate",       // required
            LastUpdatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow.AddMinutes(-1),
        };

        db.Files.Add(fileB);
        db.FileStepHistory.Add(new FileStepHistory { Id = Guid.NewGuid(), FileId = fileB.Id, StepName = "Upload", Status = "Success" });
        await db.SaveChangesAsync();

        // The CORRECT access pattern: join through Files (which IS filtered).
        var reachable = await db.FileStepHistory
            .Where(h => db.Files.Any(f => f.Id == h.FileId)) // Files auto-filtered to tenant A
            .ToListAsync();

        Assert.Empty(reachable); // B's step history is invisible to A
    }
}
