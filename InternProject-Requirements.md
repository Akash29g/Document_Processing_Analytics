# Document Processing Analytics — Intern Project Requirements

## Overview

Your company runs a cloud-based **document processing platform**. Customers upload documents (PDFs, spreadsheets, CAD files) which go through multiple processing stages before being published to a central data store. Your task is to build an **analytics and monitoring web application** that gives operations teams visibility into the health and status of this pipeline.

You are **not** given a database schema, API design, or technology choices. You must analyze these requirements, design the system from scratch, and build it end to end.

---

## Business Context

- The platform serves **multiple customers** (called "tenants"). Each tenant has one or more **sites** (physical locations like factories or plants).
- Each tenant + site combination is completely isolated — one customer must never see another customer's data.
- Documents are uploaded in **batches**. A batch is a group of files submitted together as a single unit of work.
- Each file goes through a **multi-step pipeline**: Upload → Validate → Transform → Publish.
- Files can succeed, fail, or get stuck at any step.
- Operations teams need to monitor this pipeline, identify failures, and take corrective action.

---

## Functional Requirements

### FR-1: Dashboard

The main landing page should give an at-a-glance view of system health for a selected tenant and site.

**FR-1.1**: Show **summary counters** for total files in each status:
- Queued (waiting to be processed)
- In Progress (currently being processed)  
- Completed (successfully published)
- Failed (errored at any step)

**FR-1.2**: Show a **chart** of processing throughput — how many files were completed per hour/day over a configurable time range.

**FR-1.3**: Show a **chart** breaking down the current file status distribution (e.g., pie or bar chart).

**FR-1.4**: Show a **table of recent failures** with the file name, the step where it failed, the error message, and when it failed. This table should be sortable and paginated.

**FR-1.5**: The dashboard should **auto-refresh** at a configurable interval (e.g., every 30 seconds) without full page reload.

---

### FR-2: Batch Explorer

A page to browse and inspect batches and their files.

**FR-2.1**: Show a **paginated list of batches** with columns: Batch ID, status (In Progress / Completed / Failed), number of files, submission time, completion time, and source system.

**FR-2.2**: Allow **filtering** by:
- Status (all, in-progress, completed, failed)
- Date range (submitted between)
- Source system

**FR-2.3**: Allow **searching** by Batch ID.

**FR-2.4**: Clicking a batch should open a **batch detail view** showing:
- Batch summary (status, file counts per status, start/end time)
- A table of all files in the batch with: file name, current status, current step, last updated time
- For each file, the ability to drill down into step-by-step history

**FR-2.5**: The **file step history** should show a timeline/table of every processing step the file went through, with: step name, status (success/failed/skipped), timestamp, and error details (if any).

---

### FR-3: Error Analysis

A page dedicated to understanding and resolving failures.

**FR-3.1**: Show the **top 10 most frequent errors** with occurrence count, grouped by error code or error message.

**FR-3.2**: Show an **error trend chart** — number of failures per day over the last 30 days.

**FR-3.3**: For each error, show a **suggested fix** (remediation message) if one exists in the system.

**FR-3.4**: Allow **filtering errors** by:
- Date range
- Processing step where the error occurred
- Source system

**FR-3.5**: Allow **exporting** the filtered error list to CSV.

---

### FR-4: Activity Log

An audit trail of significant events in the system.

**FR-4.1**: Show a **chronological log** of events such as:
- Batch submitted
- File state changed (e.g., from "In Progress" to "Failed")
- Batch completed
- Remediation message updated

**FR-4.2**: Each log entry should include: timestamp, event type, related entity (batch ID or file name), old state, new state, and who/what triggered it.

**FR-4.3**: Allow **filtering** by event type, entity, and date range.

**FR-4.4**: The log should be **paginated** (not load all records at once).

---

### FR-5: Tenant & Site Selection

**FR-5.1**: The application must support multiple tenants and sites. Provide a way for the user to select which tenant and site they are viewing.

**FR-5.2**: All data displayed across every page must be scoped to the selected tenant + site. Switching tenant/site should reload all data.

**FR-5.3**: A user should not be able to access data from a tenant/site they are not authorized for.

---

## Non-Functional Requirements

### NFR-1: Performance
- Dashboard page must load within **3 seconds** with up to 1 million file records in the database.
- Paginated lists must return results within **1 second** for page sizes up to 50.
- The system should handle **10 concurrent users** without degradation.

### NFR-2: Usability
- The application must be **responsive** and usable on screens from 1024px to 1920px wide.
- Use consistent navigation (sidebar or top nav) across all pages.
- Show **loading indicators** when data is being fetched.
- Show **user-friendly error messages** when API calls fail.

### NFR-3: Security
- All API endpoints must require **authentication** (token-based).
- All database queries must enforce **tenant isolation** — a user must only see their own tenant's data.
- No raw SQL concatenation — all queries must be parameterized.
- API inputs must be validated (e.g., date ranges, page sizes, IDs).

### NFR-4: Reliability
- The application must include a **health check endpoint** that verifies database connectivity.
- Failed API calls on the frontend should show a retry option or a meaningful error — not a blank screen.

### NFR-5: Maintainability
- Code should follow **separation of concerns** — data access, business logic, and API/presentation layers should be distinct.
- Use a **migration-based approach** for database schema management (not manual DDL scripts).
- API responses should follow a **consistent format** (e.g., `{ data: ..., error: ..., pagination: ... }`).

---

## Design Tasks (Before You Write Code)

Complete these design exercises before building. Document your decisions.

### DT-1: Data Modeling
- What **entities** (tables) do you need? What are their columns and data types?
- What are the **relationships** between entities? (one-to-many, many-to-many)
- What **indexes** will you create and why?
- How will you enforce **tenant isolation** at the database level?
- How will you track **file step history** — one row per step, or a JSON array?

### DT-2: API Design
- List all your **API endpoints** with HTTP method, URL, request parameters, and response shape.
- How will you handle **pagination**? (offset-based, cursor-based)
- How will you handle **filtering and sorting**? (query parameters, request body)
- How will you structure **error responses**?
- How will you handle **authentication** on each request?

### DT-3: Frontend Architecture
- What **pages/routes** will your application have?
- What **reusable components** can you identify? (e.g., status badges appear everywhere)
- How will you manage **state**? (signals, services, NgRx, or simple observables)
- How will you handle the **tenant/site selection** globally?
- How will you implement **auto-refresh** on the dashboard?

### DT-4: Performance Thinking
- If the Files table has 1 million rows, how will your "file state distribution" query perform?
- Should you **pre-aggregate** counts or compute them on the fly?
- What data is safe to **cache** and for how long?
- How will you avoid the **N+1 query problem** when loading a batch with its files?

---

## Evaluation Criteria

Your project will be assessed on:

| Criteria | Weight | What We Look For |
|----------|--------|-----------------|
| **Data Model Design** | 20% | Normalized schema, appropriate indexes, tenant isolation, relationships |
| **API Design** | 20% | RESTful conventions, consistent responses, pagination, error handling |
| **Frontend Implementation** | 20% | Component architecture, routing, state management, UX quality |
| **Code Quality** | 15% | Separation of concerns, naming conventions, no hardcoded values, testability |
| **Performance Awareness** | 10% | Indexed queries, pagination, caching strategy, no unnecessary data loading |
| **Security** | 10% | Auth enforcement, tenant isolation, input validation, parameterized queries |
| **Documentation** | 5% | Design decisions recorded, README with setup instructions, API documented |

---

## Constraints & Guidelines

- **You choose the technology stack.** Suggested options (pick one per layer):
  - Backend: ASP.NET Core, Node.js (Express/NestJS), Python (FastAPI/Django), Java (Spring Boot)
  - Frontend: Angular, React, Vue
  - Database: PostgreSQL, MySQL, SQL Server
  - ORM: Entity Framework Core, Prisma, SQLAlchemy, TypeORM, Sequelize

- You must use a **relational database** (not MongoDB or similar). The goal is to practice relational design.

- Start with the **database design** before writing any code. Get it reviewed before proceeding.

- Build the **API layer** before the frontend. Verify it works with Postman or Swagger before connecting the UI.

- Use **seed data** to populate your database with realistic test data (at least 100 batches, 500+ files across multiple tenants).

---

## Stretch Goals (Optional)

If you finish early, consider adding:

- **S-1**: WebSocket/SignalR live updates — push file state changes to the dashboard in real time instead of polling.
- **S-2**: Dark mode toggle with persisted preference.
- **S-3**: Role-based access — Admin can see all tenants, Viewer can only see assigned tenant.
- **S-4**: Email notification configuration — let users set up alerts when failure rate exceeds a threshold.
- **S-5**: File processing time percentiles — P50, P90, P99 processing times per step.
- **S-6**: Comparison view — compare throughput between two date ranges side by side.

---

## Getting Started

1. Read all requirements above thoroughly
2. Complete **DT-1 through DT-4** (design tasks) and get them reviewed
3. Set up your database and create your schema via migrations
4. Write a seed script to generate test data
5. Build and test your API endpoints (use Swagger or Postman)
6. Build the frontend, connecting one page at a time
7. Demo your working application
