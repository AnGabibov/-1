PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS Projects (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    Name        TEXT    NOT NULL UNIQUE,
    CreatedAt   TEXT    NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS TimeLogs (
    Id               INTEGER PRIMARY KEY AUTOINCREMENT,
    ProjectId        INTEGER NOT NULL,
    StartedAt        TEXT    NOT NULL,
    EndedAt          TEXT,
    DurationSeconds  INTEGER CHECK (DurationSeconds >= 0),
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE,
    CHECK (EndedAt IS NULL OR EndedAt >= StartedAt)
);

CREATE INDEX IF NOT EXISTS idx_logs_project ON TimeLogs(ProjectId);
CREATE INDEX IF NOT EXISTS idx_logs_started ON TimeLogs(StartedAt);
