PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS Room (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    Number      TEXT    NOT NULL UNIQUE,
    Name        TEXT    NOT NULL,
    Description TEXT
);

CREATE TABLE IF NOT EXISTS Employee (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    FullName    TEXT    NOT NULL,
    Position    TEXT    NOT NULL,
    Department  TEXT    NOT NULL
);

CREATE TABLE IF NOT EXISTS Equipment (
    Id              INTEGER PRIMARY KEY AUTOINCREMENT,
    Name            TEXT    NOT NULL,
    InventoryNumber TEXT    NOT NULL UNIQUE,
    SerialNumber    TEXT    UNIQUE,
    Status          INTEGER NOT NULL DEFAULT 1,
    RoomId          INTEGER REFERENCES Room(Id) ON DELETE SET NULL,
    EmployeeId      INTEGER REFERENCES Employee(Id) ON DELETE SET NULL,
    Notes           TEXT,
    CreatedAt       TEXT    NOT NULL DEFAULT (datetime('now', 'localtime')),
    UpdatedAt       TEXT    NOT NULL DEFAULT (datetime('now', 'localtime'))
);

CREATE TABLE IF NOT EXISTS InventoryCheck (
    Id                    INTEGER PRIMARY KEY AUTOINCREMENT,
    CheckDate             TEXT    NOT NULL,
    ResponsibleEmployeeId INTEGER NOT NULL REFERENCES Employee(Id),
    Comment               TEXT,
    CreatedAt             TEXT    NOT NULL DEFAULT (datetime('now', 'localtime'))
);

CREATE TABLE IF NOT EXISTS InventoryCheckItem (
    Id               INTEGER PRIMARY KEY AUTOINCREMENT,
    InventoryCheckId INTEGER NOT NULL REFERENCES InventoryCheck(Id) ON DELETE CASCADE,
    EquipmentId      INTEGER NOT NULL REFERENCES Equipment(Id),
    ResultStatus     INTEGER NOT NULL,
    Comment          TEXT
);

CREATE TABLE IF NOT EXISTS MovementHistory (
    Id             INTEGER PRIMARY KEY AUTOINCREMENT,
    EquipmentId    INTEGER NOT NULL REFERENCES Equipment(Id),
    FromRoomId     INTEGER REFERENCES Room(Id),
    ToRoomId       INTEGER REFERENCES Room(Id),
    FromEmployeeId INTEGER REFERENCES Employee(Id),
    ToEmployeeId   INTEGER REFERENCES Employee(Id),
    MovedAt        TEXT    NOT NULL DEFAULT (datetime('now', 'localtime')),
    Comment        TEXT
);
