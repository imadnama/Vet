using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using Microsoft.Data.Sqlite;

namespace ClinicVets.Data.Repositories;

public class MedicineRepository : IMedicineRepository
{
    private readonly DatabaseContext _db;

    public MedicineRepository(DatabaseContext db) => _db = db;

    public void Add(Medicine medicine)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Medicines (Name, Price, Quantity)
            VALUES ($name, $price, $quantity);
            SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$name",     medicine.Name);
        cmd.Parameters.AddWithValue("$price",    (double)medicine.Price);
        cmd.Parameters.AddWithValue("$quantity", medicine.Quantity);
        medicine.Id = Convert.ToInt32((long)cmd.ExecuteScalar()!);
    }

    public void Delete(int id)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM Medicines WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$id", id);
        cmd.ExecuteNonQuery();
    }

    public Medicine? GetById(int id)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Price, Quantity FROM Medicines WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$id", id);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapMedicine(reader) : null;
    }

    public IEnumerable<Medicine> GetAll()
    {
        var list = new List<Medicine>();
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Price, Quantity FROM Medicines ORDER BY Name;";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapMedicine(reader));
        return list;
    }

    public void Update(Medicine medicine)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE Medicines
            SET Name = $name, Price = $price, Quantity = $quantity
            WHERE Id = $id;";
        cmd.Parameters.AddWithValue("$name",     medicine.Name);
        cmd.Parameters.AddWithValue("$price",    (double)medicine.Price);
        cmd.Parameters.AddWithValue("$quantity", medicine.Quantity);
        cmd.Parameters.AddWithValue("$id",       medicine.Id);
        cmd.ExecuteNonQuery();
    }

    private static Medicine MapMedicine(SqliteDataReader r) => new()
    {
        Id       = r.GetInt32(0),
        Name     = r.GetString(1),
        Price    = (decimal)r.GetDouble(2),
        Quantity = r.GetInt32(3),
    };
}
