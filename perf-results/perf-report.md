# Performance Report (mocked, in-memory)

Generated: 2026-07-10T12:34:33Z  
Dataset: 2,000 batches x 50 files

| Operation | Samples | P50 (ms) | P90 (ms) | Max (ms) |
|---|---:|---:|---:|---:|
| batch_list_page | 10 | 1.1 | 2.9 | 20.8 |
| concurrent_dashboard_summary | 10 | 1.5 | 23.2 | 156.5 |
| concurrent_total_wall_time | 1 | 211.4 | 211.4 | 211.4 |
| dashboard_summary | 10 | 1.4 | 3.4 | 20.8 |
| error_list_page | 10 | 168.3 | 258.4 | 301.4 |
| recent_failures_page | 10 | 61.4 | 153.3 | 163.6 |
