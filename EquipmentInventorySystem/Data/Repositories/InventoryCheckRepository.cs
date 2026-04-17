using EquipmentInventorySystem.Models;
using Microsoft.Data.Sqlite;

namespace EquipmentInventorySystem.Data.Repositories;

public class InventoryCheckRepository
{
    public List<InventoryCheck> GetAll()
    {
        var list = new List<InventoryCheck>();

        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, CheckDate, ResponsibleEmployeeId, Comment, CreatedAt
            FROM InventoryCheck
            ORDER BY CheckDate DESC;
            """;

        using var reader = command.ExecuteReader();
        while (reader.Read())
            list.Add(MapCheck(reader));

        return list;
    }

    public InventoryCheck? GetById(int id)
    {
        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, CheckDate, ResponsibleEmployeeId, Comment, CreatedAt
            FROM InventoryCheck
            WHERE Id = @Id;
            """;
        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapCheck(reader) : null;
    }

    public List<InventoryCheckItem> GetItemsByCheckId(int checkId)
    {
        var list = new List<InventoryCheckItem>();

        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, InventoryCheckId, EquipmentId, ResultStatus, Comment
            FROM InventoryCheckItem
            WHERE InventoryCheckId = @CheckId;
            """;
        command.Parameters.AddWithValue("@CheckId", checkId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
            list.Add(MapCheckItem(reader));

        return list;
    }

    /// <summary>
    /// Saves the check header and all its items in a single transaction.
    /// Returns the assigned Id of the created InventoryCheck.
    /// </summary>
    public int Save(InventoryCheck check, IEnumerable<InventoryCheckItem> items)
    {
        using var connection = DatabaseHelper.OpenConnection();
        using var transaction = connection.BeginTransaction();

        int checkId;

        try
        {
            checkId = InsertCheck(connection, transaction, check);

            foreach (var item in items)
            {
                item.InventoryCheckId = checkId;
                InsertCheckItem(connection, transaction, item);
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }

        return checkId;
    }

    private static int InsertCheck(SqliteConnection connection, SqliteTransaction transaction, InventoryCheck check)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO InventoryCheck (CheckDate, ResponsibleEmployeeId, Comment, CreatedAt)
            VALUES (@CheckDate, @ResponsibleEmployeeId, @Comment, @CreatedAt);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("@CheckDate", check.CheckDate.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("@ResponsibleEmployeeId", check.ResponsibleEmployeeId);
        command.Parameters.AddWithValue("@Comment", (object?)check.Comment ?? DBNull.Value);
        command.Parameters.AddWithValue("@CreatedAt", check.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));

        return Convert.ToInt32(command.ExecuteScalar());
    }

    private static void InsertCheckItem(SqliteConnection connection, SqliteTransaction transaction, InventoryCheckItem item)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO InventoryCheckItem (InventoryCheckId, EquipmentId, ResultStatus, Comment)
            VALUES (@InventoryCheckId, @EquipmentId, @ResultStatus, @Comment);
            """;
        command.Parameters.AddWithValue("@InventoryCheckId", item.InventoryCheckId);
        command.Parameters.AddWithValue("@EquipmentId", item.EquipmentId);
        command.Parameters.AddWithValue("@ResultStatus", (int)item.ResultStatus);
        command.Parameters.AddWithValue("@Comment", (object?)item.Comment ?? DBNull.Value);

        command.ExecuteNonQuery();
    }

    public List<InventoryCheckDisplayRow> GetAllWithDetails()
    {
        var list = new List<InventoryCheckDisplayRow>();

        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT ic.Id, ic.CheckDate, ic.Comment, ic.CreatedAt,
                   COALESCE(emp.FullName, 'Не назначено') AS ResponsibleEmployee,
                   (SELECT COUNT(*) FROM InventoryCheckItem ici WHERE ici.InventoryCheckId = ic.Id) AS ItemCount
            FROM InventoryCheck ic
            LEFT JOIN Employee emp ON emp.Id = ic.ResponsibleEmployeeId
            ORDER BY ic.CheckDate DESC;
            """;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new InventoryCheckDisplayRow
            {
                Id                    = reader.GetInt32(reader.GetOrdinal("Id")),
                CheckDate             = DateTime.Parse(reader.GetString(reader.GetOrdinal("CheckDate"))),
                ResponsibleEmployee   = reader.GetString(reader.GetOrdinal("ResponsibleEmployee")),
                ItemCount             = reader.GetInt32(reader.GetOrdinal("ItemCount")),
                Comment               = reader.IsDBNull(reader.GetOrdinal("Comment"))
                                            ? null
                                            : reader.GetString(reader.GetOrdinal("Comment")),
                CreatedAt             = DateTime.Parse(reader.GetString(reader.GetOrdinal("CreatedAt")))
            });
        }

        return list;
    }

    public List<InventoryCheckItemDetailRow> GetItemsWithDetails(int checkId)
    {
        var list = new List<InventoryCheckItemDetailRow>();

        using var connection = DatabaseHelper.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT e.Name AS EquipmentName,
                   e.InventoryNumber,
                   COALESCE(e.SerialNumber, '')                              AS SerialNumber,
                   COALESCE(r.Number || ' – ' || r.Name, 'Не назначено')    AS RoomDisplay,
                   COALESCE(emp.FullName, 'Не назначено')                   AS EmployeeDisplay,
                   ici.ResultStatus,
                   COALESCE(ici.Comment, '')                                AS Comment
            FROM InventoryCheckItem ici
            JOIN Equipment e   ON e.Id   = ici.EquipmentId
            LEFT JOIN Room r   ON r.Id   = e.RoomId
            LEFT JOIN Employee emp ON emp.Id = e.EmployeeId
            WHERE ici.InventoryCheckId = @CheckId
            ORDER BY e.Name;
            """;
        command.Parameters.AddWithValue("@CheckId", checkId);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var status = (InventoryResultStatus)reader.GetInt32(reader.GetOrdinal("ResultStatus"));
            list.Add(new InventoryCheckItemDetailRow
            {
                EquipmentName       = reader.GetString(reader.GetOrdinal("EquipmentName")),
                InventoryNumber     = reader.GetString(reader.GetOrdinal("InventoryNumber")),
                SerialNumber        = reader.GetString(reader.GetOrdinal("SerialNumber")),
                RoomDisplay         = reader.GetString(reader.GetOrdinal("RoomDisplay")),
                EmployeeDisplay     = reader.GetString(reader.GetOrdinal("EmployeeDisplay")),
                ResultStatusDisplay = InventoryResultStatusHelper.GetDisplay(status),
                Comment             = reader.GetString(reader.GetOrdinal("Comment"))
            });
        }

        return list;
    }

    private static InventoryCheck MapCheck(SqliteDataReader reader) => new()
    {
        Id                    = reader.GetInt32(reader.GetOrdinal("Id")),
        CheckDate             = DateTime.Parse(reader.GetString(reader.GetOrdinal("CheckDate"))),
        ResponsibleEmployeeId = reader.GetInt32(reader.GetOrdinal("ResponsibleEmployeeId")),
        Comment               = reader.IsDBNull(reader.GetOrdinal("Comment"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("Comment")),
        CreatedAt             = DateTime.Parse(reader.GetString(reader.GetOrdinal("CreatedAt")))
    };

    private static InventoryCheckItem MapCheckItem(SqliteDataReader reader) => new()
    {
        Id               = reader.GetInt32(reader.GetOrdinal("Id")),
        InventoryCheckId = reader.GetInt32(reader.GetOrdinal("InventoryCheckId")),
        EquipmentId      = reader.GetInt32(reader.GetOrdinal("EquipmentId")),
        ResultStatus     = (InventoryResultStatus)reader.GetInt32(reader.GetOrdinal("ResultStatus")),
        Comment          = reader.IsDBNull(reader.GetOrdinal("Comment"))
                               ? null
                               : reader.GetString(reader.GetOrdinal("Comment"))
    };
}
