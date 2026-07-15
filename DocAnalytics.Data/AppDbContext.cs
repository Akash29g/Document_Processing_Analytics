using DocAnalytics.Domain.Common;
using DocAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;

namespace DocAnalytics.Data;

public class AppDbContext : DbContext, IDataProtectionKeyContext
{
    private readonly ICurrentUser _currentUser;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUser currentUser)
        : base(options) => _currentUser = currentUser;

    public virtual DbSet<Tenant> Tenants => Set<Tenant>();
    public virtual DbSet<Site> Sites => Set<Site>();
    public virtual DbSet<User> Users => Set<User>();
    public virtual DbSet<UserSiteAccess> UserSiteAccess => Set<UserSiteAccess>();
    public virtual DbSet<Transaction> Transactions => Set<Transaction>();
    public virtual DbSet<FileRecord> Files => Set<FileRecord>();
    public virtual DbSet<FileStepHistory> FileStepHistory => Set<FileStepHistory>();
    public virtual DbSet<ErrorCatalog> ErrorCatalog => Set<ErrorCatalog>();
    public virtual DbSet<ActivityLog> ActivityLog => Set<ActivityLog>();
    public virtual DbSet<DocumentType> DocumentTypes => Set<DocumentType>();
    public virtual DbSet<InvoiceLineItem> InvoiceLineItems => Set<InvoiceLineItem>();
    public virtual DbSet<ItemCategory> ItemCategories => Set<ItemCategory>();
    public virtual DbSet<AlertRule> AlertRules => Set<AlertRule>();

    public virtual DbSet<AlertNotification> AlertNotifications => Set<AlertNotification>();

    public virtual DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    public virtual DbSet<LoginAttempt> LoginAttempts => Set<LoginAttempt>();




    public virtual DbSet<InvoiceHeader> InvoiceHeaders => Set<InvoiceHeader>();


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

        // ---- table names ----
        b.Entity<AlertRule>().ToTable("alert_rules");

        // ---- constraints / indexes ----
        b.Entity<AlertRule>().Property(a => a.Name).HasMaxLength(120);
        b.Entity<AlertRule>().Property(a => a.Email).HasMaxLength(400);
        b.Entity<AlertRule>().HasIndex(a => new { a.TenantId, a.SiteId });


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

        b.Entity<InvoiceHeader>(e =>
        {
            e.Property(p => p.Subtotal).HasPrecision(12, 2);
            e.Property(p => p.Discount).HasPrecision(12, 2);
            e.Property(p => p.Tax).HasPrecision(12, 2);
            e.Property(p => p.Shipping).HasPrecision(12, 2);
            e.Property(p => p.Total).HasPrecision(12, 2);
            e.HasOne(p => p.File).WithMany().HasForeignKey(p => p.FileId);
        });

        b.Entity<AlertNotification>(e =>
        {
            e.Property(x => x.RuleName).HasMaxLength(120).IsRequired();
            e.Property(x => x.Message).HasMaxLength(500).IsRequired();
            e.Property(x => x.Severity).HasMaxLength(20).IsRequired();

            // Same index shape as your other tenant-scoped read tables:
            // unread-first, newest-first, scoped to tenant+site.
            e.HasIndex(x => new { x.TenantId, x.SiteId, x.IsRead, x.FiredAt })
             .HasDatabaseName("ix_alert_notifications_tenant_site_read_fired");

            // Tenant isolation — identical pattern to AlertRule/Transaction/File.
            e.HasQueryFilter(x => x.TenantId == _currentUser.TenantId
                               && x.SiteId == _currentUser.SiteId);
        });

        // feature/roles-schema
        b.Entity<Tenant>(e =>
        {
            e.Property(t => t.OrgDomain).HasMaxLength(100).IsRequired();
            e.HasIndex(t => t.OrgDomain).IsUnique();
        });

        b.Entity<User>(e =>
        {
            // role whitelist at the DB level
            e.ToTable(t => t.HasCheckConstraint(
                "ck_users_role", "role IN ('Developer','Admin','Viewer')"));
        });

        // ---- login brute-force tracker (pre-auth: NOT tenant-scoped) ----
        b.Entity<LoginAttempt>(e =>
        {
            e.ToTable("login_attempts");
            e.HasKey(x => x.Id);
            e.Property(x => x.Email).HasMaxLength(400).IsRequired();
            e.Property(x => x.Ip).HasMaxLength(64);
            e.HasIndex(x => x.Email).IsUnique();   // one running counter per account
        });

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
