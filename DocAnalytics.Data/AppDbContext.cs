using System.Linq.Expressions;
using System.Reflection;
using DocAnalytics.Domain.Common;
using DocAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocAnalytics.Data;

public class AppDbContext : DbContext
{
    private readonly ICurrentUser _currentUser;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser currentUser)
        : base(options) => _currentUser = currentUser;

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSiteAccess> UserSiteAccess => Set<UserSiteAccess>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<FileRecord> Files => Set<FileRecord>();
    public DbSet<FileStepHistory> FileStepHistory => Set<FileStepHistory>();
    public DbSet<ErrorCatalog> ErrorCatalog => Set<ErrorCatalog>();
    public DbSet<ActivityLog> ActivityLog => Set<ActivityLog>();
    public DbSet<DocumentType> DocumentTypes => Set<DocumentType>();
    public DbSet<InvoiceLineItem> InvoiceLineItems => Set<InvoiceLineItem>();
    public DbSet<ItemCategory> ItemCategories => Set<ItemCategory>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // ---- table names (match DT-1) ----
        b.Entity<FileRecord>().ToTable("files");
        b.Entity<FileStepHistory>().ToTable("file_step_history");
        b.Entity<ErrorCatalog>().ToTable("error_catalog");
        b.Entity<ActivityLog>().ToTable("activity_log");
        b.Entity<InvoiceLineItem>().ToTable("invoice_line_items");
        b.Entity<ItemCategory>().ToTable("item_categories");

        // ---- precision ----
        b.Entity<FileRecord>().Property(f => f.ExtractionConfidence).HasPrecision(4, 3);
        b.Entity<InvoiceLineItem>().Property(i => i.Quantity).HasPrecision(12, 3);
        b.Entity<InvoiceLineItem>().Property(i => i.UnitPrice).HasPrecision(12, 2);
        b.Entity<InvoiceLineItem>().Property(i => i.LineTotal).HasPrecision(12, 2);
        b.Entity<InvoiceLineItem>().Property(i => i.Confidence).HasPrecision(4, 3);

        // ---- uniqueness ----
        b.Entity<User>().HasIndex(u => u.Email).IsUnique();
        b.Entity<ErrorCatalog>().HasIndex(e => e.ErrorCode).IsUnique();
        b.Entity<DocumentType>().HasIndex(d => d.TypeName).IsUnique();
        b.Entity<ItemCategory>().HasIndex(c => c.CategoryCode).IsUnique();
        b.Entity<UserSiteAccess>().HasIndex(x => new { x.UserId, x.SiteId }).IsUnique();

        // ---- performance indexes (DT-1) ----
        b.Entity<Transaction>().HasIndex(t => new { t.TenantId, t.SiteId, t.LastUpdatedAt });
        b.Entity<Transaction>().HasIndex(t => new { t.TenantId, t.SiteId, t.State });
        b.Entity<FileRecord>().HasIndex(f => f.TransactionId);
        b.Entity<FileRecord>().HasIndex(f => new { f.TenantId, f.SiteId, f.Status, f.LastUpdatedAt });
        b.Entity<FileRecord>().HasIndex(f => new { f.TenantId, f.SiteId, f.DocumentTypeId });
        b.Entity<FileStepHistory>().HasIndex(s => s.FileId);
        b.Entity<FileStepHistory>().HasIndex(s => new { s.StepName, s.Status });
        b.Entity<InvoiceLineItem>().HasIndex(i => i.FileId);
        b.Entity<InvoiceLineItem>().HasIndex(i => new { i.TenantId, i.SiteId, i.ItemCategoryId });
        b.Entity<ActivityLog>().HasIndex(a => new { a.TenantId, a.SiteId, a.CreatedAt });

        // ---- relationships ----
        b.Entity<Site>().HasOne(s => s.Tenant).WithMany(t => t.Sites)
            .HasForeignKey(s => s.TenantId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<User>().HasOne(u => u.Tenant).WithMany(t => t.Users)
            .HasForeignKey(u => u.TenantId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<FileRecord>().HasOne(f => f.Transaction).WithMany(t => t.Files)
            .HasForeignKey(f => f.TransactionId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<FileStepHistory>().HasOne(s => s.File).WithMany(f => f.Steps)
            .HasForeignKey(s => s.FileId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<InvoiceLineItem>().HasOne(i => i.File).WithMany(f => f.LineItems)
            .HasForeignKey(i => i.FileId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<InvoiceLineItem>().HasOne(i => i.ItemCategory).WithMany(c => c.LineItems)
            .HasForeignKey(i => i.ItemCategoryId).OnDelete(DeleteBehavior.SetNull);

        // ---- GLOBAL TENANT/SITE FILTER (every ITenantScoped entity) ----
        foreach (var et in b.Model.GetEntityTypes())
        {
            if (typeof(ITenantScoped).IsAssignableFrom(et.ClrType))
            {
                var method = typeof(AppDbContext)
                    .GetMethod(nameof(BuildTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(et.ClrType);
                b.Entity(et.ClrType).HasQueryFilter((LambdaExpression)method.Invoke(this, null)!);
            }
        }
    }

    // _currentUser is re-evaluated as a parameter at query time (model cached once).
    private LambdaExpression BuildTenantFilter<TEntity>() where TEntity : class, ITenantScoped
    {
        Expression<Func<TEntity, bool>> filter =
            e => e.TenantId == _currentUser.TenantId && e.SiteId == _currentUser.SiteId;
        return filter;
    }
}
