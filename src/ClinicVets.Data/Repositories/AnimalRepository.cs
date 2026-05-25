using ClinicVets.Core.Enums;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using Microsoft.Data.Sqlite;

namespace ClinicVets.Data.Repositories;

public class AnimalRepository : IAnimalRepository
{
    private readonly DatabaseContext _db;

    public AnimalRepository(DatabaseContext db) => _db = db;

    public Animal? GetByChipNumber(string chipNumber)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = SelectAll() + " WHERE ChipNumber = @chip";
        cmd.Parameters.AddWithValue("@chip", chipNumber);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapAnimal(reader) : null;
    }

    public IEnumerable<Animal> SearchByName(string name)
    {
        var list = new List<Animal>();
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = SelectAll() + " WHERE Name LIKE @name";
        cmd.Parameters.AddWithValue("@name", $"%{name}%");
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapAnimal(reader));
        return list;
    }

    // Used by CustomerService.GetCustomerAnimals to list all animals for a customer.
    public IEnumerable<Animal> GetByOwnerId(int ownerId)
    {
        var list = new List<Animal>();
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = SelectAll() + " WHERE OwnerId = @ownerId";
        cmd.Parameters.AddWithValue("@ownerId", ownerId);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapAnimal(reader));
        return list;
    }

    public void Add(Animal animal)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Animals (ChipNumber, Name, Type, Weight, BirthDate, OwnerId, LastVaccinationDate)
            VALUES (@chip, @name, @type, @weight, @birth, @owner, @vaccine)";
        cmd.Parameters.AddWithValue("@chip",    animal.ChipNumber);
        cmd.Parameters.AddWithValue("@name",    animal.Name);
        cmd.Parameters.AddWithValue("@type",    (int)animal.Type);
        cmd.Parameters.AddWithValue("@weight",  (double)animal.Weight);
        cmd.Parameters.AddWithValue("@birth",   animal.BirthDate.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("@owner",   animal.OwnerId);
        cmd.Parameters.AddWithValue("@vaccine", animal.LastVaccinationDate.HasValue
            ? (object)animal.LastVaccinationDate.Value.ToString("yyyy-MM-dd")
            : DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    public void Update(Animal animal)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE Animals
               SET ChipNumber=@chip, Name=@name, Type=@type, Weight=@weight,
                   BirthDate=@birth, OwnerId=@owner, LastVaccinationDate=@vaccine
             WHERE Id=@id";
        cmd.Parameters.AddWithValue("@chip",    animal.ChipNumber);
        cmd.Parameters.AddWithValue("@name",    animal.Name);
        cmd.Parameters.AddWithValue("@type",    (int)animal.Type);
        cmd.Parameters.AddWithValue("@weight",  (double)animal.Weight);
        cmd.Parameters.AddWithValue("@birth",   animal.BirthDate.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("@owner",   animal.OwnerId);
        cmd.Parameters.AddWithValue("@vaccine", animal.LastVaccinationDate.HasValue
            ? (object)animal.LastVaccinationDate.Value.ToString("yyyy-MM-dd")
            : DBNull.Value);
        cmd.Parameters.AddWithValue("@id", animal.Id);
        cmd.ExecuteNonQuery();
    }

    public IEnumerable<Animal> GetAll()
    {
        var list = new List<Animal>();
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = SelectAll();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapAnimal(reader));
        return list;
    }

    private static string SelectAll() =>
        "SELECT Id, ChipNumber, Name, Type, Weight, BirthDate, OwnerId, LastVaccinationDate FROM Animals";

    private static Animal MapAnimal(SqliteDataReader r) => new()
    {
        Id                  = r.GetInt32(0),
        ChipNumber          = r.GetString(1),
        Name                = r.GetString(2),
        Type                = (AnimalType)r.GetInt32(3),
        Weight              = (decimal)r.GetDouble(4),
        BirthDate           = DateTime.Parse(r.GetString(5)),
        OwnerId             = r.GetInt32(6),
        LastVaccinationDate = r.IsDBNull(7) ? null : DateTime.Parse(r.GetString(7)),
    };
}
