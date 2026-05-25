using ClinicVets.Core.Enums;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;
using Microsoft.Data.Sqlite;

namespace ClinicVets.Data.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly DatabaseContext _db;

    public EmployeeRepository(DatabaseContext db) => _db = db;

    public Employee? GetByUsername(string username)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            "SELECT Id, Username, PasswordHash, EmployeeNumber, Email, NationalId, FullName, Role " +
            "FROM Employees WHERE Username = @u";
        cmd.Parameters.AddWithValue("@u", username);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapEmployee(reader) : null;
    }

    public bool UsernameExists(string username)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(1) FROM Employees WHERE Username = @u";
        cmd.Parameters.AddWithValue("@u", username);
        return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
    }

    public bool EmployeeNumberExists(string employeeNumber)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(1) FROM Employees WHERE EmployeeNumber = @n";
        cmd.Parameters.AddWithValue("@n", employeeNumber);
        return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
    }

    public void Add(Employee employee)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Employees (Username, PasswordHash, EmployeeNumber, Email, NationalId, FullName, Role)
            VALUES (@user, @hash, @empNum, @email, @natId, @fullName, @role)";
        cmd.Parameters.AddWithValue("@user",    employee.Username);
        cmd.Parameters.AddWithValue("@hash",    employee.PasswordHash);
        cmd.Parameters.AddWithValue("@empNum",  employee.EmployeeNumber);
        cmd.Parameters.AddWithValue("@email",   employee.Email);
        cmd.Parameters.AddWithValue("@natId",   employee.NationalId);
        cmd.Parameters.AddWithValue("@fullName", employee.FullName);
        cmd.Parameters.AddWithValue("@role",    (int)employee.Role);
        cmd.ExecuteNonQuery();
    }

    public IEnumerable<Employee> GetAll()
    {
        var list = new List<Employee>();
        using var conn = _db.CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            "SELECT Id, Username, PasswordHash, EmployeeNumber, Email, NationalId, FullName, Role " +
            "FROM Employees";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(MapEmployee(reader));
        return list;
    }

    private static Employee MapEmployee(SqliteDataReader r) => new()
    {
        Id             = r.GetInt32(0),
        Username       = r.GetString(1),
        PasswordHash   = r.GetString(2),
        EmployeeNumber = r.GetString(3),
        Email          = r.GetString(4),
        NationalId     = r.GetString(5),
        FullName       = r.GetString(6),
        Role           = (Role)r.GetInt32(7),
    };
}
