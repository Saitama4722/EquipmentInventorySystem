using Microsoft.Data.Sqlite;

namespace EquipmentInventorySystem.Data;

public static class DatabaseHelper
{
    private const string DatabaseFileName = "inventory.db";

    public static string ConnectionString =>
        $"Data Source={DatabaseFileName}";

    public static SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var pragma = connection.CreateCommand();
        pragma.CommandText = "PRAGMA foreign_keys = ON;";
        pragma.ExecuteNonQuery();

        return connection;
    }
}
