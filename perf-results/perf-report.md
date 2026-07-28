# Performance Report (mocked, in-memory)

Generated: 2026-07-28T07:13:44Z  
Dataset: 2,000 batches x 50 files

| Operation | Samples | P50 (ms) | P90 (ms) | Max (ms) |
|---|---:|---:|---:|---:|
| batch_list_page | 10 | 0.8 | 3.0 | 30.8 |
| concurrent_dashboard_summary | 10 | 1.1 | 12.7 | 25.1 |
| concurrent_total_wall_time | 1 | 54.4 | 54.4 | 54.4 |
| dashboard_summary | 10 | 1.2 | 21.8 | 101.1 |
| error_list_page | 10 | 196.3 | 306.2 | 530.3 |
| recent_failures_page | 10 | 58.8 | 191.4 | 228.5 |
