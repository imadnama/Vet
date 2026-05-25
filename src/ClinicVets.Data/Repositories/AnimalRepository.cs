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
        cmd.CommandText = @"
            SELECT Id, ChipNumber, Name, Type, Weight, BirthDate, OwnerId, LastVaccinationDate
            FROM Animals
            WHERE ChipNumber = $chipNumber;";
        cmd.Parameters.AddWithValue("$chipNumber", chipNumber);

        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapAnimal(reader) : null;
    }

    public IEnumerable<Animal> SearchByName(string name)
    {
        var animals = new List<Animal>();

        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT Id, ChipNumber, Name, Type, Weight, BirthDate, OwnerId, LastVaccinationDate
            FROM Animals
            WHERE Name LIKE $name
            ORDER BY Name, ChipNumber;";
        cmd.Parameters.AddWithValue("$name", $"%{name}%");

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            animals.Add(MapAnimal(reader));
        }

        return animals;
    }

    public IEnumerable<Animal> GetByOwnerId(int ownerId)
    {
        var animals = new List<Animal>();

        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT Id, ChipNumber, Name, Type, Weight, BirthDate, OwnerId, LastVaccinationDate
            FROM Animals
            WHERE OwnerId = $ownerId
            ORDER BY Name, ChipNumber;";
        cmd.Parameters.AddWithValue("$ownerId", ownerId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            animals.Add(MapAnimal(reader));
        }

        return animals;
    }

    public void Add(Animal animal)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Animals (ChipNumber, Name, Type, Weight, BirthDate, OwnerId, LastVaccinationDate)
            VALUES ($chipNumber, $name, $type, $weight, $birthDate, $ownerId, $lastVaccinationDate);
            SELECT last_insert_rowid();";
        AddAnimalParameters(cmd, animal);

        animal.Id = Convert.ToInt32((long)cmd.ExecuteScalar()!);
    }

    public void Update(Animal animal)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE Animals
            SET ChipNumber = $chipNumber,
                Name = $name,
                Type = $type,
                Weight = $weight,
                BirthDate = $birthDate,
                OwnerId = $ownerId,
                LastVaccinationDate = $lastVaccinationDate
            WHERE Id = $id;";
        AddAnimalParameters(cmd, animal);
        cmd.Parameters.AddWithValue("$id", animal.Id);
        cmd.ExecuteNonQuery();
    }

    public IEnumerable<Animal> GetAll()
    {
        var animals = new List<Animal>();

        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT Id, ChipNumber, Name, Type, Weight, BirthDate, OwnerId, LastVaccinationDate
            FROM Animals
            ORDER BY Name, ChipNumber;";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            animals.Add(MapAnimal(reader));
        }

        return animals;
    }

    private static void AddAnimalParameters(SqliteCommand cmd, Animal animal)
    {
        cmd.Parameters.AddWithValue("$chipNumber", animal.ChipNumber);
        cmd.Parameters.AddWithValue("$name", animal.Name);
        cmd.Parameters.AddWithValue("$type", (int)animal.Type);
        cmd.Parameters.AddWithValue("$weight", animal.Weight);
        cmd.Parameters.AddWithValue("$birthDate", animal.BirthDate.ToString("O"));
        cmd.Parameters.AddWithValue("$ownerId", animal.OwnerId);
        cmd.Parameters.AddWithValue(
            "$lastVaccinationDate",
            animal.LastVaccinationDate.HasValue
                ? animal.LastVaccinationDate.Value.ToString("O")
                : DBNull.Value);
    }

    private static Animal MapAnimal(SqliteDataReader reader)
        => new()
        {
            Id = reader.GetInt32(0),
            ChipNumber = reader.GetString(1),
            Name = reader.GetString(2),
            Type = (AnimalType)reader.GetInt32(3),
            Weight = reader.GetDecimal(4),
            BirthDate = DateTime.Parse(reader.GetString(5)),
            OwnerId = reader.GetInt32(6),
            LastVaccinationDate = reader.IsDBNull(7) ? null : DateTime.Parse(reader.GetString(7))
        };
}
