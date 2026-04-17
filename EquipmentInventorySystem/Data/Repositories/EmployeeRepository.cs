using EquipmentInventorySystem.Models;
using Microsoft.Data.Sqlite;

namespace EquipmentInventorySystem.Data.Repositories;

public class EmployeeRepository
{
    public List<Employee> GetAll()
    {
        var employees = new List<Employee>();

        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, FullName, Position, Department FROM Employee ORDER BY FullName;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
            employees.Add(MapEmployee(reader));

        return employees;
    }

    public Employee? GetById(int id)
    {
        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, FullName, Position, Department FROM Employee WHERE Id = @Id;";
        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapEmployee(reader) : null;
    }

    public int Add(Employee employee)
    {
        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Employee (FullName, Position, Department)
            VALUES (@FullName, @Position, @Department);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("@FullName", employee.FullName);
        command.Parameters.AddWithValue("@Position", employee.Position);
        command.Parameters.AddWithValue("@Department", employee.Department);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void Update(Employee employee)
    {
        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Employee SET FullName = @FullName, Position = @Position, Department = @Department
            WHERE Id = @Id;
            """;
        command.Parameters.AddWithValue("@Id", employee.Id);
        command.Parameters.AddWithValue("@FullName", employee.FullName);
        command.Parameters.AddWithValue("@Position", employee.Position);
        command.Parameters.AddWithValue("@Department", employee.Department);

        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Employee WHERE Id = @Id;";
        command.Parameters.AddWithValue("@Id", id);

        command.ExecuteNonQuery();
    }

    public List<EmployeeSummaryRow> GetSummaryWithEquipmentCount()
    {
        var list = new List<EmployeeSummaryRow>();

        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT emp.FullName, emp.Position, emp.Department,
                   COUNT(e.Id) AS EquipmentCount
            FROM Employee emp
            LEFT JOIN Equipment e ON e.EmployeeId = emp.Id
            GROUP BY emp.Id, emp.FullName, emp.Position, emp.Department
            ORDER BY emp.FullName;
            """;

        using var reader = command.ExecuteReader();
        while (reader.Read())
            list.Add(new EmployeeSummaryRow
            {
                FullName       = reader.GetString(reader.GetOrdinal("FullName")),
                Position       = reader.GetString(reader.GetOrdinal("Position")),
                Department     = reader.GetString(reader.GetOrdinal("Department")),
                EquipmentCount = reader.GetInt32(reader.GetOrdinal("EquipmentCount"))
            });

        return list;
    }

    private static Employee MapEmployee(SqliteDataReader reader) => new()
    {
        Id         = reader.GetInt32(reader.GetOrdinal("Id")),
        FullName   = reader.GetString(reader.GetOrdinal("FullName")),
        Position   = reader.GetString(reader.GetOrdinal("Position")),
        Department = reader.GetString(reader.GetOrdinal("Department"))
    };
}
