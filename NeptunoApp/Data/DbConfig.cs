using Microsoft.Data.SqlClient;

namespace NeptunoApp.Data;

public static class DbConfig
{
    public static string GetConnectionString()
    {
        var complete = Environment.GetEnvironmentVariable("NEPTUNO_CONNECTION_STRING");
        if (!string.IsNullOrWhiteSpace(complete)) return complete;

        var password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD");
        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Configura MSSQL_SA_PASSWORD o NEPTUNO_CONNECTION_STRING antes de iniciar la aplicación.");

        return new SqlConnectionStringBuilder
        {
            DataSource = $"{Environment.GetEnvironmentVariable("MSSQL_HOST") ?? "localhost"},{Environment.GetEnvironmentVariable("MSSQL_PORT") ?? "1433"}",
            InitialCatalog = "NeptunoDB",
            UserID = Environment.GetEnvironmentVariable("MSSQL_USER") ?? "sa",
            Password = password,
            Encrypt = true,
            TrustServerCertificate = true,
            ConnectTimeout = 8
        }.ConnectionString;
    }
}

