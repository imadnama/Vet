using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using Microsoft.Data.Sqlite;

namespace ClinicVets.Data.Repositories;

public class VisitRepository : IVisitRepository
{
    private readonly DatabaseContext _db;

    public VisitRepository(DatabaseContext db) => _db = db;

    public void Add(Visit visit)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();

        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = @"
            INSERT INTO Visits (AnimalId, Reason, VisitDateTime, Diagnosis, VetEmployeeId, TotalCost)
            VALUES ($animalId, $reason, $dateTime, $diagnosis, $vetId, $cost);
            SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$animalId",  visit.AnimalId);
        cmd.Parameters.AddWithValue("$reason",    visit.Reason);
        cmd.Parameters.AddWithValue("$dateTime",  visit.VisitDateTime.ToString("O"));
        cmd.Parameters.AddWithValue("$diagnosis", visit.Diagnosis);
        cmd.Parameters.AddWithValue("$vetId",     visit.VetEmployeeId);
        cmd.Parameters.AddWithValue("$cost",      (double)visit.TotalCost);
        visit.Id = Convert.ToInt32((long)cmd.ExecuteScalar()!);

        foreach (var med in visit.Medicines)
        {
            using var jCmd = conn.CreateCommand();
            jCmd.Transaction = tx;
            jCmd.CommandText = @"
                INSERT OR IGNORE INTO VisitMedicines (VisitId, MedicineId)
                VALUES ($visitId, $medId);";
            jCmd.Parameters.AddWithValue("$visitId", visit.Id);
            jCmd.Parameters.AddWithValue("$medId",   med.Id);
            jCmd.ExecuteNonQuery();
        }

        tx.Commit();
    }

    public IEnumerable<Visit> GetAll()
    {
        var visits = new List<Visit>();
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT Id, AnimalId, Reason, VisitDateTime, Diagnosis, VetEmployeeId, TotalCost
            FROM Visits
            ORDER BY VisitDateTime DESC;";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            visits.Add(MapVisit(reader));
        reader.Close();
        foreach (var visit in visits)
            visit.Medicines = LoadMedicines(conn, visit.Id);
        return visits;
    }

    public Visit? GetById(int id)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT Id, AnimalId, Reason, VisitDateTime, Diagnosis, VetEmployeeId, TotalCost
            FROM Visits WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$id", id);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;
        var visit = MapVisit(reader);
        reader.Close();
        visit.Medicines = LoadMedicines(conn, visit.Id);
        return visit;
    }

    public IEnumerable<Visit> GetByAnimalId(int animalId)
    {
        var visits = new List<Visit>();
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT Id, AnimalId, Reason, VisitDateTime, Diagnosis, VetEmployeeId, TotalCost
            FROM Visits WHERE AnimalId = $animalId
            ORDER BY VisitDateTime DESC;";
        cmd.Parameters.AddWithValue("$animalId", animalId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            visits.Add(MapVisit(reader));
        reader.Close();

        foreach (var visit in visits)
            visit.Medicines = LoadMedicines(conn, visit.Id);

        return visits;
    }

    public void Update(Visit visit)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();

        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = @"
            UPDATE Visits
            SET AnimalId = $animalId, Reason = $reason, VisitDateTime = $dateTime,
                Diagnosis = $diagnosis, VetEmployeeId = $vetId, TotalCost = $cost
            WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$animalId",  visit.AnimalId);
        cmd.Parameters.AddWithValue("$reason",    visit.Reason);
        cmd.Parameters.AddWithValue("$dateTime",  visit.VisitDateTime.ToString("O"));
        cmd.Parameters.AddWithValue("$diagnosis", visit.Diagnosis);
        cmd.Parameters.AddWithValue("$vetId",     visit.VetEmployeeId);
        cmd.Parameters.AddWithValue("$cost",      (double)visit.TotalCost);
        cmd.Parameters.AddWithValue("$id",        visit.Id);
        cmd.ExecuteNonQuery();

        using var delCmd = conn.CreateCommand();
        delCmd.Transaction = tx;
        delCmd.CommandText = "DELETE FROM VisitMedicines WHERE VisitId = $visitId;";
        delCmd.Parameters.AddWithValue("$visitId", visit.Id);
        delCmd.ExecuteNonQuery();

        foreach (var med in visit.Medicines)
        {
            using var jCmd = conn.CreateCommand();
            jCmd.Transaction = tx;
            jCmd.CommandText = @"
                INSERT OR IGNORE INTO VisitMedicines (VisitId, MedicineId)
                VALUES ($visitId, $medId);";
            jCmd.Parameters.AddWithValue("$visitId", visit.Id);
            jCmd.Parameters.AddWithValue("$medId",   med.Id);
            jCmd.ExecuteNonQuery();
        }

        tx.Commit();
    }

    private static List<Medicine> LoadMedicines(SqliteConnection conn, int visitId)
    {
        var medicines = new List<Medicine>();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT m.Id, m.Name, m.Price, m.Quantity
            FROM Medicines m
            JOIN VisitMedicines vm ON vm.MedicineId = m.Id
            WHERE vm.VisitId = $visitId;";
        cmd.Parameters.AddWithValue("$visitId", visitId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            medicines.Add(new Medicine
            {
                Id       = reader.GetInt32(0),
                Name     = reader.GetString(1),
                Price    = (decimal)reader.GetDouble(2),
                Quantity = reader.GetInt32(3),
            });
        return medicines;
    }

    private static Visit MapVisit(SqliteDataReader r) => new()
    {
        Id            = r.GetInt32(0),
        AnimalId      = r.GetInt32(1),
        Reason        = r.GetString(2),
        VisitDateTime = DateTime.Parse(r.GetString(3)),
        Diagnosis     = r.GetString(4),
        VetEmployeeId = r.GetInt32(5),
        TotalCost     = (decimal)r.GetDouble(6),
    };
}
