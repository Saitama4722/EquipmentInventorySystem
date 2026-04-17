using EquipmentInventorySystem.Models;
using Microsoft.Data.Sqlite;

namespace EquipmentInventorySystem.Data.Repositories;

public class EquipmentRepository
{
    public List<EquipmentDisplayRow> GetAllForDisplay(string? search, EquipmentStatus? statusFilter)
    {
        var list = new List<EquipmentDisplayRow>();

        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();

        var where = new List<string>();

        if (!string.IsNullOrWhiteSpace(search))
            where.Add("(e.Name LIKE @Query OR e.InventoryNumber LIKE @Query OR COALESCE(e.SerialNumber,'') LIKE @Query)");

        if (statusFilter.HasValue)
            where.Add("e.Status = @Status");

        string whereClause = where.Count > 0
            ? "WHERE " + string.Join(" AND ", where)
            : string.Empty;

        command.CommandText = $"""
            SELECT e.Id, e.Name, e.InventoryNumber,
                   COALESCE(e.SerialNumber, '')   AS SerialNumber,
                   e.Status,
                   COALESCE(e.Notes, '')           AS Notes,
                   COALESCE(r.Number || ' – ' || r.Name, 'Не назначено') AS RoomDisplay,
                   COALESCE(emp.FullName, 'Не назначено')                AS EmployeeDisplay
            FROM Equipment e
            LEFT JOIN Room     r   ON r.Id   = e.RoomId
            LEFT JOIN Employee emp ON emp.Id = e.EmployeeId
            {whereClause}
            ORDER BY e.Name;
            """;

        if (!string.IsNullOrWhiteSpace(search))
            command.Parameters.AddWithValue("@Query", $"%{search}%");

        if (statusFilter.HasValue)
            command.Parameters.AddWithValue("@Status", (int)statusFilter.Value);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var status = (EquipmentStatus)reader.GetInt32(reader.GetOrdinal("Status"));
            list.Add(new EquipmentDisplayRow
            {
                Id              = reader.GetInt32(reader.GetOrdinal("Id")),
                Name            = reader.GetString(reader.GetOrdinal("Name")),
                InventoryNumber = reader.GetString(reader.GetOrdinal("InventoryNumber")),
                SerialNumber    = reader.GetString(reader.GetOrdinal("SerialNumber")),
                Status          = status,
                StatusDisplay   = EquipmentStatusHelper.GetDisplay(status),
                RoomDisplay     = reader.GetString(reader.GetOrdinal("RoomDisplay")),
                EmployeeDisplay = reader.GetString(reader.GetOrdinal("EmployeeDisplay")),
                Notes           = reader.GetString(reader.GetOrdinal("Notes"))
            });
        }

        return list;
    }


    public List<Equipment> GetAll()
    {
        var list = new List<Equipment>();

        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Name, InventoryNumber, SerialNumber, Status,
                   RoomId, EmployeeId, Notes, CreatedAt, UpdatedAt
            FROM Equipment
            ORDER BY Name;
            """;

        using var reader = command.ExecuteReader();
        while (reader.Read())
            list.Add(MapEquipment(reader));

        return list;
    }

    public Equipment? GetById(int id)
    {
        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Name, InventoryNumber, SerialNumber, Status,
                   RoomId, EmployeeId, Notes, CreatedAt, UpdatedAt
            FROM Equipment
            WHERE Id = @Id;
            """;
        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapEquipment(reader) : null;
    }

    public List<Equipment> Search(string query)
    {
        var list = new List<Equipment>();

        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Name, InventoryNumber, SerialNumber, Status,
                   RoomId, EmployeeId, Notes, CreatedAt, UpdatedAt
            FROM Equipment
            WHERE Name LIKE @Query
               OR InventoryNumber LIKE @Query
               OR SerialNumber LIKE @Query
            ORDER BY Name;
            """;
        command.Parameters.AddWithValue("@Query", $"%{query}%");

        using var reader = command.ExecuteReader();
        while (reader.Read())
            list.Add(MapEquipment(reader));

        return list;
    }

    public int Add(Equipment equipment)
    {
        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Equipment
                (Name, InventoryNumber, SerialNumber, Status, RoomId, EmployeeId, Notes, CreatedAt, UpdatedAt)
            VALUES
                (@Name, @InventoryNumber, @SerialNumber, @Status, @RoomId, @EmployeeId, @Notes,
                 @CreatedAt, @UpdatedAt);
            SELECT last_insert_rowid();
            """;

        BindEquipmentParams(command, equipment);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void Update(Equipment equipment)
    {
        equipment.UpdatedAt = DateTime.Now;

        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Equipment SET
                Name            = @Name,
                InventoryNumber = @InventoryNumber,
                SerialNumber    = @SerialNumber,
                Status          = @Status,
                RoomId          = @RoomId,
                EmployeeId      = @EmployeeId,
                Notes           = @Notes,
                UpdatedAt       = @UpdatedAt
            WHERE Id = @Id;
            """;
        command.Parameters.AddWithValue("@Id", equipment.Id);
        command.Parameters.AddWithValue("@Name", equipment.Name);
        command.Parameters.AddWithValue("@InventoryNumber", equipment.InventoryNumber);
        command.Parameters.AddWithValue("@SerialNumber", (object?)equipment.SerialNumber ?? DBNull.Value);
        command.Parameters.AddWithValue("@Status", (int)equipment.Status);
        command.Parameters.AddWithValue("@RoomId", (object?)equipment.RoomId ?? DBNull.Value);
        command.Parameters.AddWithValue("@EmployeeId", (object?)equipment.EmployeeId ?? DBNull.Value);
        command.Parameters.AddWithValue("@Notes", (object?)equipment.Notes ?? DBNull.Value);
        command.Parameters.AddWithValue("@UpdatedAt", equipment.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));

        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Equipment WHERE Id = @Id;";
        command.Parameters.AddWithValue("@Id", id);

        command.ExecuteNonQuery();
    }

    private static void BindEquipmentParams(SqliteCommand command, Equipment e)
    {
        command.Parameters.AddWithValue("@Name", e.Name);
        command.Parameters.AddWithValue("@InventoryNumber", e.InventoryNumber);
        command.Parameters.AddWithValue("@SerialNumber", (object?)e.SerialNumber ?? DBNull.Value);
        command.Parameters.AddWithValue("@Status", (int)e.Status);
        command.Parameters.AddWithValue("@RoomId", (object?)e.RoomId ?? DBNull.Value);
        command.Parameters.AddWithValue("@EmployeeId", (object?)e.EmployeeId ?? DBNull.Value);
        command.Parameters.AddWithValue("@Notes", (object?)e.Notes ?? DBNull.Value);
        command.Parameters.AddWithValue("@CreatedAt", e.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        command.Parameters.AddWithValue("@UpdatedAt", e.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
    }

    private static Equipment MapEquipment(SqliteDataReader reader) => new()
    {
        Id              = reader.GetInt32(reader.GetOrdinal("Id")),
        Name            = reader.GetString(reader.GetOrdinal("Name")),
        InventoryNumber = reader.GetString(reader.GetOrdinal("InventoryNumber")),
        SerialNumber    = NullableString(reader, "SerialNumber"),
        Status          = (EquipmentStatus)reader.GetInt32(reader.GetOrdinal("Status")),
        RoomId          = NullableInt(reader, "RoomId"),
        EmployeeId      = NullableInt(reader, "EmployeeId"),
        Notes           = NullableString(reader, "Notes"),
        CreatedAt       = DateTime.Parse(reader.GetString(reader.GetOrdinal("CreatedAt"))),
        UpdatedAt       = DateTime.Parse(reader.GetString(reader.GetOrdinal("UpdatedAt")))
    };

    private static int? NullableInt(SqliteDataReader reader, string column)
    {
        int ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
    }

    private static string? NullableString(SqliteDataReader reader, string column)
    {
        int ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }
}
