namespace DocAnalytics.Service.ActivityLog;

// query-string params (FR-4.3 filters + paging/sort)
public sealed class ActivityLogQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? EventType { get; set; }   // exact, e.g. FILE_STATE_CHANGED | BATCH_SUBMITTED
    public string? EntityType { get; set; }  // exact, e.g. File | Batch
    public string? Entity { get; set; }      // partial match on entity_name
    public DateTime? From { get; set; }       // created on/after  (ISO-8601)
    public DateTime? To { get; set; }         // created on/before (ISO-8601)
    public string? SortBy { get; set; }       // ts | event_type | entity
    public string? SortDir { get; set; }      // asc | desc (default desc → newest first)
}

// one audit row (FR-4.2)
public sealed class ActivityLogItemDto
{
    public DateTime Ts { get; set; }            // → "ts"          (CreatedAt)
    public string EventType { get; set; } = null!;
    public string EntityType { get; set; } = null!;
    public string? Entity { get; set; }         // → "entity"      (EntityName)
    public string? OldState { get; set; }
    public string? NewState { get; set; }
    public string Actor { get; set; } = null!;  // → "actor"       (TriggeredBy)
}
