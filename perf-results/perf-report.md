# Performance Report (mocked, in-memory)

Generated: 2026-07-23T04:08:58Z  
Dataset: 2,000 batches x 50 files

| Operation | Samples | P50 (ms) | P90 (ms) | Max (ms) |
|---|---:|---:|---:|---:|
| batch_list_page | 10 | 0.8 | 2.6 | 29.6 |
| concurrent_dashboard_summary | 10 | 1.1 | 13.0 | 22.5 |
| concurrent_total_wall_time | 1 | 52.3 | 52.3 | 52.3 |
| dashboard_summary | 10 | 1.1 | 3.2 | 20.5 |
| error_list_page | 10 | 151.9 | 275.1 | 418.8 |
| recent_failures_page | 10 | 57.6 | 197.9 | 287.9 |
