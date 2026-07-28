# Performance Report (mocked, in-memory)

Generated: 2026-07-27T11:32:21Z  
Dataset: 2,000 batches x 50 files

| Operation | Samples | P50 (ms) | P90 (ms) | Max (ms) |
|---|---:|---:|---:|---:|
| batch_list_page | 10 | 0.9 | 3.2 | 23.3 |
| concurrent_dashboard_summary | 10 | 1.1 | 12.9 | 23.9 |
| concurrent_total_wall_time | 1 | 53.7 | 53.7 | 53.7 |
| dashboard_summary | 10 | 1.4 | 21.8 | 103.0 |
| error_list_page | 10 | 197.1 | 318.3 | 497.7 |
| recent_failures_page | 10 | 62.9 | 189.4 | 230.4 |
