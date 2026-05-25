using Microsoft.Data.Sqlite;

namespace ClinicVets.Data;

/// <summary>
/// Owns the SQLite connection string and ensures all tables exist at startup.
/// Every repository receives this via constructor injection.
/// </summary>
public class DatabaseContext
{
    private readonly string _connectionString;

    public DatabaseContext(string dbPath = "clinicvets.db")
    {
        _connectionString = $"Data Source={dbPath}";
        InitializeSchema();
    }

    public SqliteConnection CreateConnection() => new(_connectionString);

    private void InitializeSchema()
    {
        using var conn = CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Employees (
                Id             INTEGER PRIMARY KEY AUTOINCREMENT,
                Username       TEXT    NOT NULL UNIQUE,
                PasswordHash   TEXT    NOT NULL,
                EmployeeNumber TEXT    NOT NULL UNIQUE,
                Email          TEXT    NOT NULL,
                NationalId     TEXT    NOT NULL,
                FullName       TEXT    NOT NULL,
                Role           INTEGER NOT NULL
            );
            CREATE TABLE IF NOT EXISTS Customers (
                Id         INTEGER PRIMARY KEY AUTOINCREMENT,
                FullName   TEXT NOT NULL,
                NationalId TEXT NOT NULL UNIQUE,
                Phone      TEXT NOT NULL,
                Email      TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS Animals (
                Id                  INTEGER PRIMARY KEY AUTOINCREMENT,
                ChipNumber          TEXT    NOT NULL UNIQUE,
                Name                TEXT    NOT NULL,
                Type                INTEGER NOT NULL,
                Weight              REAL    NOT NULL,
                BirthDate           TEXT    NOT NULL,
                OwnerId             INTEGER NOT NULL,
                LastVaccinationDate TEXT,
                FOREIGN KEY(OwnerId) REFERENCES Customers(Id)
            );
            CREATE TABLE IF NOT EXISTS Medicines (
                Id       INTEGER PRIMARY KEY AUTOINCREMENT,
                Name     TEXT NOT NULL,
                Price    REAL NOT NULL,
                Quantity INTEGER NOT NULL
            );
            CREATE TABLE IF NOT EXISTS Visits (
                Id            INTEGER PRIMARY KEY AUTOINCREMENT,
                AnimalId      INTEGER NOT NULL,
                Reason        TEXT    NOT NULL,
                VisitDateTime TEXT    NOT NULL,
                Diagnosis     TEXT    NOT NULL DEFAULT '',
                VetEmployeeId INTEGER NOT NULL,
                TotalCost     REAL    NOT NULL DEFAULT 0,
                FOREIGN KEY(AnimalId)      REFERENCES Animals(Id),
                FOREIGN KEY(VetEmployeeId) REFERENCES Employees(Id)
            );
            CREATE TABLE IF NOT EXISTS VisitMedicines (
                VisitId    INTEGER NOT NULL,
                MedicineId INTEGER NOT NULL,
                PRIMARY KEY(VisitId, MedicineId),
                FOREIGN KEY(VisitId)    REFERENCES Visits(Id),
                FOREIGN KEY(MedicineId) REFERENCES Medicines(Id)
            );
        ";
        cmd.ExecuteNonQuery();
    }
}
