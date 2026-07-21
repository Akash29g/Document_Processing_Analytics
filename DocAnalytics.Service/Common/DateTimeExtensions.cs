namespace DocAnalytics.Service.Common;

/// <summary>Helpers for normalizing <see cref="DateTime"/> values before use in PostgreSQL queries.</summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Normalizes a value to UTC for use with Postgres <c>timestamptz</c> (Npgsql rejects Kind=Unspecified).
    /// Local values are converted; Unspecified values are treated as UTC.
    /// </summary>
    /// <param name="dt">The value to normalize.</param>
    /// <returns>An equivalent value with Kind=Utc.</returns>
    // Postgres 'timestamptz' requires UTC. Query-string dates parse as Kind=Unspecified,
    // which Npgsql rejects — so normalise to UTC before using in a query.
    public static DateTime AsUtc(this DateTime dt) => dt.Kind switch
    {
        DateTimeKind.Utc => dt,
        DateTimeKind.Local => dt.ToUniversalTime(),
        _ => DateTime.SpecifyKind(dt, DateTimeKind.Utc) // Unspecified → treat as UTC
    };
}
