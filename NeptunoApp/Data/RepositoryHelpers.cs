using System.Data;
using Microsoft.Data.SqlClient;

namespace NeptunoApp.Data;
internal static class RepositoryHelpers
{
    public static SqlCommand Procedure(string name, SqlConnection connection) => new(name, connection) { CommandType = CommandType.StoredProcedure };
    public static object Db(string? value) => string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim();
    public static object Db<T>(T? value) where T : struct => value.HasValue ? value.Value : DBNull.Value;
    public static string? Text(SqlDataReader r, string name) => r.IsDBNull(r.GetOrdinal(name)) ? null : r.GetString(r.GetOrdinal(name));
    public static int? Int(SqlDataReader r, string name) => r.IsDBNull(r.GetOrdinal(name)) ? null : r.GetInt32(r.GetOrdinal(name));
    public static DateTime? Date(SqlDataReader r, string name) => r.IsDBNull(r.GetOrdinal(name)) ? null : r.GetDateTime(r.GetOrdinal(name));
}

