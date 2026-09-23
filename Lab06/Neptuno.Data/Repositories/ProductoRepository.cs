using System.Data;
using Microsoft.Data.SqlClient;
using Neptuno.Entities;

namespace Neptuno.Data;

public sealed class ProductoRepository(string connectionString) : IProductoRepository
{
    public async Task<List<Producto>> ListarAsync()
    {
        return await Task.Run(() => {
            using var connection = new SqlConnection(connectionString);
            using var command = StoredProcedure("dbo.usp_Producto_Listar", connection);
            using var da = new SqlDataAdapter(command);
            var dt = new DataTable();
            da.Fill(dt);
            var result = new List<Producto>();
            foreach(DataRow row in dt.Rows) result.Add(Map(row));
            return result;
        });
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
        return await Task.Run(() => {
            using var connection = new SqlConnection(connectionString);
            using var command = StoredProcedure("dbo.usp_Categoria_Listar", connection);
            using var da = new SqlDataAdapter(command);
            var dt = new DataTable();
            da.Fill(dt);
            var result = new List<Categoria>();
            foreach(DataRow r in dt.Rows) result.Add(new(Convert.ToInt32(r["CategoriaID"]), Convert.ToString(r["NombreCategoria"])));
            return result;
        });
    }

    public async Task<List<Proveedor>> ListarProveedoresAsync()
    {
        return await Task.Run(() => {
            using var connection = new SqlConnection(connectionString);
            using var command = StoredProcedure("dbo.usp_Proveedor_Listar", connection);
            using var da = new SqlDataAdapter(command);
            var dt = new DataTable();
            da.Fill(dt);
            var result = new List<Proveedor>();
            foreach(DataRow r in dt.Rows) result.Add(new(Convert.ToInt32(r["ProveedorID"]), Convert.ToString(r["NombreCompania"])));
            return result;
        });
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

    private static Producto Map(DataRow r) => new()
    {
        ProductoID = Convert.ToInt32(r["ProductoID"]),
        NombreProducto = Convert.ToString(r["NombreProducto"]),
        ProveedorID = r["ProveedorID"] == DBNull.Value ? null : Convert.ToInt32(r["ProveedorID"]),
        CategoriaID = r["CategoriaID"] == DBNull.Value ? null : Convert.ToInt32(r["CategoriaID"]),
        CantidadPorUnidad = r["CantidadPorUnidad"] == DBNull.Value ? null : Convert.ToString(r["CantidadPorUnidad"]),
        PrecioUnidad = Convert.ToDecimal(r["PrecioUnidad"]),
        UnidadesEnExistencia = Convert.ToInt16(r["UnidadesEnExistencia"]),
        UnidadesEnPedido = Convert.ToInt16(r["UnidadesEnPedido"]),
        NivelDeReorden = Convert.ToInt16(r["NivelDeReorden"]),
        Descontinuado = Convert.ToBoolean(r["Descontinuado"]),
        NombreCategoria = r["NombreCategoria"] == DBNull.Value ? null : Convert.ToString(r["NombreCategoria"]),
        NombreProveedor = r["NombreProveedor"] == DBNull.Value ? null : Convert.ToString(r["NombreProveedor"])
    };
}
