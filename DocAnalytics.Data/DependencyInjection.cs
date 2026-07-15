using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace DocAnalytics.Data;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(cfg.GetConnectionString("Default"))
               .UseSnakeCaseNamingConvention()
               .ConfigureWarnings(w => w.Ignore(
                   CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning)));
        return services;
    }
}
