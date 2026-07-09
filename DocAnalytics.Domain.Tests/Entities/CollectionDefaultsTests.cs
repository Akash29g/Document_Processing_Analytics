using DocAnalytics.Domain.Entities;

namespace DocAnalytics.Domain.Tests.Entities;

public class CollectionDefaultsTests
{
    // Guards against NREs: navigation collections must be initialised, not null.
    [Fact]
    public void Tenant_collections_are_initialised()
    {
        var t = new Tenant();
        Assert.NotNull(t.Sites);
        Assert.NotNull(t.Users);
        Assert.Empty(t.Sites);
    }

    [Fact]
    public void Transaction_files_is_initialised()
    {
        Assert.NotNull(new Transaction().Files);
        Assert.Empty(new Transaction().Files);
    }

    [Fact]
    public void ItemCategory_lineitems_is_initialised()
        => Assert.NotNull(new ItemCategory().LineItems);

    [Fact]
    public void User_site_access_is_initialised()
        => Assert.NotNull(new User().SiteAccess);
}
