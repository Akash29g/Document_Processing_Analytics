# Performance Report (mocked, in-memory)

Generated: 2026-07-22T05:40:57Z  
Dataset: 2,000 batches x 50 files

| Operation | Samples | P50 (ms) | P90 (ms) | Max (ms) |
|---|---:|---:|---:|---:|
| batch_list_page | 10 | 1.1 | 3.1 | 16.1 |
| concurrent_dashboard_summary | 10 | 1.5 | 14.7 | 23.9 |
| concurrent_total_wall_time | 1 | 58.8 | 58.8 | 58.8 |
| dashboard_summary | 10 | 1.5 | 4.1 | 21.9 |
| error_list_page | 10 | 161.0 | 261.7 | 317.1 |
| recent_failures_page | 10 | 55.8 | 172.5 | 176.7 |
