namespace DocAnalytics.Service.Common;

public static class DateTimeExtensions
{
    // Postgres 'timestamptz' requires UTC. Query-string dates parse as Kind=Unspecified,
    // which Npgsql rejects — so normalise to UTC before using in a query.
    public static DateTime AsUtc(this DateTime dt) => dt.Kind switch
    {
        DateTimeKind.Utc => dt,
        DateTimeKind.Local => dt.ToUniversalTime(),
        _ => DateTime.SpecifyKind(dt, DateTimeKind.Utc) // Unspecified → treat as UTC
    };
}
