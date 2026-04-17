using EquipmentInventorySystem.Models;
using Microsoft.Data.Sqlite;

namespace EquipmentInventorySystem.Data.Repositories;

public class MovementHistoryRepository
{
    public List<MovementHistoryDisplayRow> GetAllWithDetails()
    {
        var list = new List<MovementHistoryDisplayRow>();
        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT
                mh.Id,
                mh.MovedAt,
                mh.Comment,
                e.Name            AS EquipmentName,
                e.InventoryNumber,
                (fr.Number || ' – ' || fr.Name) AS FromRoomDisplay,
                (tr.Number || ' – ' || tr.Name) AS ToRoomDisplay,
                fe.FullName       AS FromEmployeeDisplay,
                te.FullName       AS ToEmployeeDisplay
            FROM MovementHistory mh
            JOIN  Equipment e  ON e.Id  = mh.EquipmentId
            LEFT JOIN Room     fr ON fr.Id = mh.FromRoomId
            LEFT JOIN Room     tr ON tr.Id = mh.ToRoomId
            LEFT JOIN Employee fe ON fe.Id = mh.FromEmployeeId
            LEFT JOIN Employee te ON te.Id = mh.ToEmployeeId
            ORDER BY mh.MovedAt DESC;
            """;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new MovementHistoryDisplayRow
            {
                Id                  = reader.GetInt32(reader.GetOrdinal("Id")),
                MovedAt             = DateTime.Parse(reader.GetString(reader.GetOrdinal("MovedAt"))),
                Comment             = NullableString(reader, "Comment"),
                EquipmentName       = reader.GetString(reader.GetOrdinal("EquipmentName")),
                InventoryNumber     = reader.GetString(reader.GetOrdinal("InventoryNumber")),
                FromRoomDisplay     = NullableString(reader, "FromRoomDisplay"),
                ToRoomDisplay       = NullableString(reader, "ToRoomDisplay"),
                FromEmployeeDisplay = NullableString(reader, "FromEmployeeDisplay"),
                ToEmployeeDisplay   = NullableString(reader, "ToEmployeeDisplay")
            });
        }
        return list;
    }

    private static string? NullableString(SqliteDataReader reader, string column)
    {
        int ord = reader.GetOrdinal(column);
        return reader.IsDBNull(ord) ? null : reader.GetString(ord);
    }

    public int Add(MovementHistory entry)
    {
        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO MovementHistory
                (EquipmentId, FromRoomId, ToRoomId, FromEmployeeId, ToEmployeeId, MovedAt, Comment)
            VALUES
                (@EquipmentId, @FromRoomId, @ToRoomId, @FromEmployeeId, @ToEmployeeId, @MovedAt, @Comment);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("@EquipmentId", entry.EquipmentId);
        command.Parameters.AddWithValue("@FromRoomId", (object?)entry.FromRoomId ?? DBNull.Value);
        command.Parameters.AddWithValue("@ToRoomId", (object?)entry.ToRoomId ?? DBNull.Value);
        command.Parameters.AddWithValue("@FromEmployeeId", (object?)entry.FromEmployeeId ?? DBNull.Value);
        command.Parameters.AddWithValue("@ToEmployeeId", (object?)entry.ToEmployeeId ?? DBNull.Value);
        command.Parameters.AddWithValue("@MovedAt", entry.MovedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        command.Parameters.AddWithValue("@Comment", (object?)entry.Comment ?? DBNull.Value);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public List<MovementHistory> GetByEquipmentId(int equipmentId)
    {
        var list = new List<MovementHistory>();

        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, EquipmentId, FromRoomId, ToRoomId, FromEmployeeId, ToEmployeeId, MovedAt, Comment
            FROM MovementHistory
            WHERE EquipmentId = @EquipmentId
            ORDER BY MovedAt DESC;
            """;
        command.Parameters.AddWithValue("@EquipmentId", equipmentId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
            list.Add(MapEntry(reader));

        return list;
    }

    private static MovementHistory MapEntry(SqliteDataReader reader) => new()
    {
        Id             = reader.GetInt32(reader.GetOrdinal("Id")),
        EquipmentId    = reader.GetInt32(reader.GetOrdinal("EquipmentId")),
        FromRoomId     = NullableInt(reader, "FromRoomId"),
        ToRoomId       = NullableInt(reader, "ToRoomId"),
        FromEmployeeId = NullableInt(reader, "FromEmployeeId"),
        ToEmployeeId   = NullableInt(reader, "ToEmployeeId"),
        MovedAt        = DateTime.Parse(reader.GetString(reader.GetOrdinal("MovedAt"))),
        Comment        = reader.IsDBNull(reader.GetOrdinal("Comment"))
                             ? null
                             : reader.GetString(reader.GetOrdinal("Comment"))
    };

    private static int? NullableInt(SqliteDataReader reader, string column)
    {
        int ordinal = reader.GetOrdinal(column);
        return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
    }
}
