-- =============================================================================
-- create-dgstool-auditlog.sql
--
-- Creates dbo.DgsToolAuditLog in the external DGSDataTest database.
--
-- This is a ONE-TIME MANUAL step - it is NOT run automatically by the app
-- (unlike DgsTool's own local database, which is migrated/seeded on every
-- start via SeedDataService). Run this script directly against DGSDataTest
-- BEFORE testing any Player-edit save in dgs-tool, otherwise every save will
-- fail once it tries to insert its audit rows (the Player update and its
-- audit rows are written in a single SaveChangesAsync/transaction, so a
-- missing table fails the whole save, not just the audit part).
--
-- Schema matches EXPERIENCE.md's "Audit Log Schema" section exactly:
--   - one row per changed FIELD, not one row (or JSON blob) per save action.
--   - bigint identity PK (append-friendly, avoids GUID fragmentation - this
--     table is expected to grow a lot).
--   - Password/OnlinePassword changes store the literal masked marker
--     "(cambiado)" in OldValue/NewValue, never the real value (enforced in
--     application code, not by this schema).
-- =============================================================================

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DgsToolAuditLog' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.DgsToolAuditLog
    (
        IdAuditLog          BIGINT IDENTITY(1,1) NOT NULL,
        EntityName          VARCHAR(50)    NOT NULL,
        EntityId            VARCHAR(50)    NOT NULL,
        FieldName           VARCHAR(100)   NOT NULL,
        OldValue            NVARCHAR(500)  NULL,
        NewValue            NVARCHAR(500)  NULL,
        Action              VARCHAR(20)    NOT NULL,
        ChangedByLoginName  VARCHAR(15)    NOT NULL,
        ChangedAt           DATETIME2      NOT NULL,
        CONSTRAINT PK_DgsToolAuditLog PRIMARY KEY CLUSTERED (IdAuditLog)
    );
END
GO

-- Supports the date-range filter and the report's default sort (most recent first).
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DgsToolAuditLog_ChangedAt' AND object_id = OBJECT_ID('dbo.DgsToolAuditLog'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_DgsToolAuditLog_ChangedAt
        ON dbo.DgsToolAuditLog (ChangedAt DESC);
END
GO

-- Supports "full history for this player" (entity + id filter combined).
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DgsToolAuditLog_Entity' AND object_id = OBJECT_ID('dbo.DgsToolAuditLog'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_DgsToolAuditLog_Entity
        ON dbo.DgsToolAuditLog (EntityName, EntityId, ChangedAt DESC);
END
GO

-- Supports "what did this user change".
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DgsToolAuditLog_User' AND object_id = OBJECT_ID('dbo.DgsToolAuditLog'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_DgsToolAuditLog_User
        ON dbo.DgsToolAuditLog (ChangedByLoginName, ChangedAt DESC);
END
GO
