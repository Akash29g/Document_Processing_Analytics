// Entities/Tenant.cs
namespace DocAnalytics.Domain.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public ICollection<Site> Sites { get; set; } = new List<Site>();
    public ICollection<User> Users { get; set; } = new List<User>();
}
