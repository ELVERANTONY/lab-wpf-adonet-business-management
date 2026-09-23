using System;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace Neptuno.Data;

public static class DbConfig
{
    public static string GetConnectionString()
    {
        // 1. Variable de entorno completa
        var complete = Environment.GetEnvironmentVariable("NEPTUNO_CONNECTION_STRING");
        if (!string.IsNullOrWhiteSpace(complete)) return complete;

        // 2. Parámetros de entorno individuales
        var password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD");
        if (!string.IsNullOrWhiteSpace(password))
        {
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

        // 3. App.config si existe
        try
        {
            var configConn = ConfigurationManager.ConnectionStrings["NeptunoConnection"]?.ConnectionString;
            if (!string.IsNullOrWhiteSpace(configConn)) return configConn;
        }
        catch { }

        // 4. Fallback estándar para Localhost Windows
        return "Server=localhost;Database=NeptunoDB;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=5;";
    }
}
