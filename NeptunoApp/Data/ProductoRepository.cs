using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data;

public sealed class ProductoRepository(string connectionString) : IProductoRepository
{
    public async Task<List<Producto>> ListarAsync()
    {
        await using var connection = new SqlConnection(connectionString);
        await using var command = StoredProcedure("dbo.usp_Producto_Listar", connection);
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        var result = new List<Producto>();
        while (await reader.ReadAsync()) result.Add(Map(reader));
        return result;
    }

    public async Task<int> CrearAsync(Producto producto)
    {
        await using var connection = new SqlConnection(connectionString);
        await using var command = StoredProcedure("dbo.usp_Producto_Crear", connection);
        AddProductParameters(command.Parameters, producto);
        await connection.OpenAsync();
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task ActualizarAsync(Producto producto)
    {
        await using var connection = new SqlConnection(connectionString);
        await using var command = StoredProcedure("dbo.usp_Producto_Actualizar", connection);
        command.Parameters.Add("@ProductoID", SqlDbType.Int).Value = producto.ProductoID;
        AddProductParameters(command.Parameters, producto);
        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task EliminarAsync(int productoId)
    {
        await using var connection = new SqlConnection(connectionString);
        await using var command = StoredProcedure("dbo.usp_Producto_Eliminar", connection);
        command.Parameters.Add("@ProductoID", SqlDbType.Int).Value = productoId;
        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<Categoria>> ListarCategoriasAsync()
    {
        await using var connection = new SqlConnection(connectionString);
        await using var command = StoredProcedure("dbo.usp_Categoria_Listar", connection);
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        var result = new List<Categoria>();
        while (await reader.ReadAsync()) result.Add(new(reader.GetInt32(0), reader.GetString(1)));
        return result;
    }

    public async Task<List<Proveedor>> ListarProveedoresAsync()
    {
        await using var connection = new SqlConnection(connectionString);
        await using var command = StoredProcedure("dbo.usp_Proveedor_Listar", connection);
        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        var result = new List<Proveedor>();
        while (await reader.ReadAsync()) result.Add(new(reader.GetInt32(0), reader.GetString(1)));
        return result;
    }

    private static SqlCommand StoredProcedure(string name, SqlConnection connection) => new(name, connection)
    {
        CommandType = CommandType.StoredProcedure
    };

    private static void AddProductParameters(SqlParameterCollection p, Producto x)
    {
        p.Add("@NombreProducto", SqlDbType.NVarChar, 60).Value = x.NombreProducto.Trim();
        p.Add("@ProveedorID", SqlDbType.Int).Value = (object?)x.ProveedorID ?? DBNull.Value;
        p.Add("@CategoriaID", SqlDbType.Int).Value = (object?)x.CategoriaID ?? DBNull.Value;
        p.Add("@CantidadPorUnidad", SqlDbType.NVarChar, 30).Value = string.IsNullOrWhiteSpace(x.CantidadPorUnidad) ? DBNull.Value : x.CantidadPorUnidad.Trim();
        p.Add("@PrecioUnidad", SqlDbType.Decimal).Value = x.PrecioUnidad;
        p.Add("@UnidadesEnExistencia", SqlDbType.SmallInt).Value = x.UnidadesEnExistencia;
        p.Add("@UnidadesEnPedido", SqlDbType.SmallInt).Value = x.UnidadesEnPedido;
        p.Add("@NivelDeReorden", SqlDbType.SmallInt).Value = x.NivelDeReorden;
        p.Add("@Descontinuado", SqlDbType.Bit).Value = x.Descontinuado;
    }

    private static Producto Map(SqlDataReader r) => new()
    {
        ProductoID = r.GetInt32(r.GetOrdinal("ProductoID")),
        NombreProducto = r.GetString(r.GetOrdinal("NombreProducto")),
        ProveedorID = r.IsDBNull(r.GetOrdinal("ProveedorID")) ? null : r.GetInt32(r.GetOrdinal("ProveedorID")),
        CategoriaID = r.IsDBNull(r.GetOrdinal("CategoriaID")) ? null : r.GetInt32(r.GetOrdinal("CategoriaID")),
        CantidadPorUnidad = r.IsDBNull(r.GetOrdinal("CantidadPorUnidad")) ? null : r.GetString(r.GetOrdinal("CantidadPorUnidad")),
        PrecioUnidad = r.GetDecimal(r.GetOrdinal("PrecioUnidad")),
        UnidadesEnExistencia = r.GetInt16(r.GetOrdinal("UnidadesEnExistencia")),
        UnidadesEnPedido = r.GetInt16(r.GetOrdinal("UnidadesEnPedido")),
        NivelDeReorden = r.GetInt16(r.GetOrdinal("NivelDeReorden")),
        Descontinuado = r.GetBoolean(r.GetOrdinal("Descontinuado")),
        NombreCategoria = r.IsDBNull(r.GetOrdinal("NombreCategoria")) ? null : r.GetString(r.GetOrdinal("NombreCategoria")),
        NombreProveedor = r.IsDBNull(r.GetOrdinal("NombreProveedor")) ? null : r.GetString(r.GetOrdinal("NombreProveedor"))
    };
}
