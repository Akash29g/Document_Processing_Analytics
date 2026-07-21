namespace DocAnalytics.Service.ActivityLog;

/// <summary>Query-string parameters for the activity log: filters plus paging/sort (FR-4.3).</summary>
// query-string params (FR-4.3 filters + paging/sort)
public sealed class ActivityLogQuery
{
    /// <summary>1-based page number.</summary>
    public int Page { get; set; } = 1;
    /// <summary>Page size (capped at 100).</summary>
    public int PageSize { get; set; } = 20;
    /// <summary>Exact event type filter, e.g. FILE_STATE_CHANGED | BATCH_SUBMITTED.</summary>
    public string? EventType { get; set; }   // exact, e.g. FILE_STATE_CHANGED | BATCH_SUBMITTED
    /// <summary>Exact entity type filter, e.g. File | Batch.</summary>
    public string? EntityType { get; set; }  // exact, e.g. File | Batch
    /// <summary>Partial (case-insensitive) match on the entity name.</summary>
    public string? Entity { get; set; }      // partial match on entity_name
    /// <summary>Include entries created on/after this timestamp (ISO-8601).</summary>
    public DateTime? From { get; set; }       // created on/after  (ISO-8601)
    /// <summary>Include entries created on/before this timestamp (ISO-8601).</summary>
    public DateTime? To { get; set; }         // created on/before (ISO-8601)
    /// <summary>Sort column: ts | event_type | entity.</summary>
    public string? SortBy { get; set; }       // ts | event_type | entity
    /// <summary>Sort direction: asc | desc (default desc → newest first).</summary>
    public string? SortDir { get; set; }      // asc | desc (default desc → newest first)
}

/// <summary>A single audit-trail row returned to the client (FR-4.2).</summary>
// one audit row (FR-4.2)
public sealed class ActivityLogItemDto
{
    /// <summary>Event timestamp (maps from CreatedAt).</summary>
    public DateTime Ts { get; set; }            // → "ts"          (CreatedAt)
    /// <summary>The type of event that occurred.</summary>
    public string EventType { get; set; } = null!;
    /// <summary>The type of entity the event relates to (File, Batch, …).</summary>
    public string EntityType { get; set; } = null!;
    /// <summary>The related entity's display name (maps from EntityName).</summary>
    public string? Entity { get; set; }         // → "entity"      (EntityName)
    /// <summary>The previous state, if applicable.</summary>
    public string? OldState { get; set; }
    /// <summary>The new state, if applicable.</summary>
    public string? NewState { get; set; }
    /// <summary>Who or what triggered the event (maps from TriggeredBy).</summary>
    public string Actor { get; set; } = null!;  // → "actor"       (TriggeredBy)
}
