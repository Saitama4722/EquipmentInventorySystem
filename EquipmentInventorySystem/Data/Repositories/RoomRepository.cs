using EquipmentInventorySystem.Models;
using Microsoft.Data.Sqlite;

namespace EquipmentInventorySystem.Data.Repositories;

public class RoomRepository
{
    public List<Room> GetAll()
    {
        var rooms = new List<Room>();

        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Number, Name, Description FROM Room ORDER BY Number;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
            rooms.Add(MapRoom(reader));

        return rooms;
    }

    public Room? GetById(int id)
    {
        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Number, Name, Description FROM Room WHERE Id = @Id;";
        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapRoom(reader) : null;
    }

    public int Add(Room room)
    {
        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Room (Number, Name, Description)
            VALUES (@Number, @Name, @Description);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("@Number", room.Number);
        command.Parameters.AddWithValue("@Name", room.Name);
        command.Parameters.AddWithValue("@Description", (object?)room.Description ?? DBNull.Value);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void Update(Room room)
    {
        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Room SET Number = @Number, Name = @Name, Description = @Description
            WHERE Id = @Id;
            """;
        command.Parameters.AddWithValue("@Id", room.Id);
        command.Parameters.AddWithValue("@Number", room.Number);
        command.Parameters.AddWithValue("@Name", room.Name);
        command.Parameters.AddWithValue("@Description", (object?)room.Description ?? DBNull.Value);

        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Room WHERE Id = @Id;";
        command.Parameters.AddWithValue("@Id", id);

        command.ExecuteNonQuery();
    }

    public List<RoomSummaryRow> GetSummaryWithEquipmentCount()
    {
        var list = new List<RoomSummaryRow>();

        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT r.Number, r.Name, COALESCE(r.Description, '') AS Description,
                   COUNT(e.Id) AS EquipmentCount
            FROM Room r
            LEFT JOIN Equipment e ON e.RoomId = r.Id
            GROUP BY r.Id, r.Number, r.Name, r.Description
            ORDER BY r.Number;
            """;

        using var reader = command.ExecuteReader();
        while (reader.Read())
            list.Add(new RoomSummaryRow
            {
                Number         = reader.GetString(reader.GetOrdinal("Number")),
                Name           = reader.GetString(reader.GetOrdinal("Name")),
                Description    = reader.GetString(reader.GetOrdinal("Description")),
                EquipmentCount = reader.GetInt32(reader.GetOrdinal("EquipmentCount"))
            });

        return list;
    }

    private static Room MapRoom(SqliteDataReader reader) => new()
    {
        Id          = reader.GetInt32(reader.GetOrdinal("Id")),
        Number      = reader.GetString(reader.GetOrdinal("Number")),
        Name        = reader.GetString(reader.GetOrdinal("Name")),
        Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Description"))
    };
}
