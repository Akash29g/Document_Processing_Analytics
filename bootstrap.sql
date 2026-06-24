-- ============================================================================
-- DocAnalytics - PostgreSQL bootstrap script (OPTIONAL)
-- ----------------------------------------------------------------------------
-- Purpose: create a dedicated application login role + an empty database.
--
-- You do NOT need this if you are happy connecting as the built-in 'postgres'
-- superuser and letting EF Core create the database (see README Option A).
-- Use this script if you prefer a least-privilege, dedicated app role.
--
-- IMPORTANT: This script ONLY creates the empty database and role.
--            The 12 tables, indexes and foreign keys are created by
--            EF Core migrations:
--              dotnet ef database update --project DocAnalytics.Data \
--                                        --startup-project DocAnalytics.Api
--
-- HOW TO RUN (once, as the postgres superuser):
--   psql -U postgres -h localhost -p 5432 -f db/bootstrap.sql
--
-- After running, set your connection string secret to use this role:
--   dotnet user-secrets set "ConnectionStrings:Default" \
--     "Host=localhost;Port=5432;Database=docanalytics;Username=docanalytics_app;Password=change_me_local_dev" \
--     --project DocAnalytics.Api
-- ============================================================================

-- 1) Create a dedicated login role (idempotent). CHANGE THE PASSWORD.
DO
$$
BEGIN
   IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'docanalytics_app') THEN
      CREATE ROLE docanalytics_app LOGIN PASSWORD 'change_me_local_dev';
   END IF;
END
$$;

-- Allow this role to create databases (so EF migrations can CREATE DATABASE
-- if you ever drop it). Optional but convenient for local dev.
ALTER ROLE docanalytics_app CREATEDB;

-- 2) Create the database owned by that role.
--    NOTE: CREATE DATABASE cannot run inside a transaction/DO block, and it
--    errors if the database already exists. If 'docanalytics' already exists,
--    just skip this line.
CREATE DATABASE docanalytics OWNER docanalytics_app;

-- 3) Grant privileges on the database to the app role.
GRANT ALL PRIVILEGES ON DATABASE docanalytics TO docanalytics_app;

-- Done. Now run EF Core migrations to build the schema (see header).
