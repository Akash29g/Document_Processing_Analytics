# Performance Report (mocked, in-memory)

Generated: 2026-07-22T06:04:20Z  
Dataset: 2,000 batches x 50 files

| Operation | Samples | P50 (ms) | P90 (ms) | Max (ms) |
|---|---:|---:|---:|---:|
| batch_list_page | 10 | 2.6 | 6.0 | 17.5 |
| concurrent_dashboard_summary | 10 | 1.4 | 12.9 | 24.4 |
| concurrent_total_wall_time | 1 | 57.4 | 57.4 | 57.4 |
| dashboard_summary | 10 | 2.3 | 9.0 | 21.5 |
| error_list_page | 10 | 204.1 | 332.4 | 345.8 |
| recent_failures_page | 10 | 79.8 | 204.2 | 210.7 |
