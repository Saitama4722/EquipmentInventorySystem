using System.IO;
using Microsoft.Data.Sqlite;

namespace EquipmentInventorySystem.Data;

public static class DatabaseInitializer
{
    public static void Initialize()
    {
        bool isFirstRun = !File.Exists(ResolveDbPath());

        using var connection = DatabaseHelper.OpenConnection();

        ExecuteScript(connection, LoadScript("Data/Scripts/init.sql"));

        if (isFirstRun)
        {
            ExecuteScript(connection, LoadScript("Data/Scripts/seed.sql"));
        }
    }

    private static string ResolveDbPath()
    {
        var connStr = DatabaseHelper.ConnectionString;
        // Extract the file path from "Data Source=<path>"
        const string prefix = "Data Source=";
        var path = connStr[prefix.Length..];

        return Path.IsPathRooted(path)
            ? path
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
    }

    private static string LoadScript(string relativePath)
    {
        string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
        return File.ReadAllText(fullPath);
    }

    private static void ExecuteScript(SqliteConnection connection, string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }
}
