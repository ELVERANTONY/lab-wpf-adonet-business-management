using System.Data;
using Microsoft.Data.SqlClient;
using Neptuno.Entities;
using static Neptuno.Data.RepositoryHelpers;

namespace Neptuno.Data;

public sealed class PedidoRepository(string cs) : IPedidoRepository
{
    public async Task<List<Pedido>> ListarAsync()
    {
        return await Task.Run(() =>
        {
            using var c = new SqlConnection(cs);
            using var q = Procedure("dbo.usp_Pedido_Listar", c);
            using var da = new SqlDataAdapter(q);
            var dt = new DataTable();
            da.Fill(dt);

            var a = new List<Pedido>();
            foreach (DataRow r in dt.Rows) a.Add(Map(r));
            return a;
        });
    }

    public async Task<int> CrearAsync(Pedido x)
    {
        await using var c = new SqlConnection(cs);
        await using var q = Procedure("dbo.usp_Pedido_Crear", c);
        Add(q, x);
        await c.OpenAsync();
        return Convert.ToInt32(await q.ExecuteScalarAsync());
    }

    public async Task ActualizarAsync(Pedido x)
    {
        await using var c = new SqlConnection(cs);
        await using var q = Procedure("dbo.usp_Pedido_Actualizar", c);
        q.Parameters.Add("@PedidoID", SqlDbType.Int).Value = x.PedidoID;
        Add(q, x);
        await c.OpenAsync();
        await q.ExecuteNonQueryAsync();
    }

    public async Task EliminarAsync(int id)
    {
        await using var c = new SqlConnection(cs);
        await using var q = Procedure("dbo.usp_Pedido_Eliminar", c);
        q.Parameters.Add("@PedidoID", SqlDbType.Int).Value = id;
        await c.OpenAsync();
        await q.ExecuteNonQueryAsync();
    }

    public async Task<List<Cliente>> ListarClientesAsync() => await Catalog("dbo.usp_Cliente_Listar", r => new Cliente(Convert.ToInt32(r[0]), Convert.ToString(r[1])!));
    public async Task<List<Empleado>> ListarEmpleadosAsync() => await Catalog("dbo.usp_Empleado_Listar", r => new Empleado(Convert.ToInt32(r[0]), Convert.ToString(r[1])!));
    public async Task<List<Transportista>> ListarTransportistasAsync() => await Catalog("dbo.usp_Transportista_Listar", r => new Transportista(Convert.ToInt32(r[0]), Convert.ToString(r[1])!));

    public async Task<List<LineaReporte>> ReportarAsync(DateTime inicio, DateTime fin)
    {
        return await Task.Run(() =>
        {
            using var c = new SqlConnection(cs);
            using var q = Procedure("dbo.usp_DetallePedido_ListarPorRangoFechas", c); // We'll keep the Lab04 name to not break ViewModel
            q.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = inicio.Date;
            q.Parameters.Add("@FechaFin", SqlDbType.Date).Value = fin.Date;
            
            using var da = new SqlDataAdapter(q);
            var dt = new DataTable();
            da.Fill(dt);

            var a = new List<LineaReporte>();
            foreach (DataRow r in dt.Rows)
            {
                a.Add(new()
                {
                    PedidoID = Convert.ToInt32(r[0]),
                    FechaPedido = Convert.ToDateTime(r[1]),
                    NombreCliente = Convert.ToString(r[2]),
                    NombreProducto = Convert.ToString(r[3]),
                    PrecioUnidad = Convert.ToDecimal(r[4]),
                    Cantidad = Convert.ToInt16(r[5]),
                    Descuento = Convert.ToDecimal(r[6]),
                    Subtotal = Convert.ToDecimal(r[7])
                });
            }
            return a;
        });
    }

    private async Task<List<T>> Catalog<T>(string sp, Func<DataRow, T> map)
    {
        return await Task.Run(() =>
        {
            using var c = new SqlConnection(cs);
            using var q = Procedure(sp, c);
            using var da = new SqlDataAdapter(q);
            var dt = new DataTable();
            da.Fill(dt);
            var a = new List<T>();
            foreach (DataRow r in dt.Rows) a.Add(map(r));
            return a;
        });
    }

    private static void Add(SqlCommand q, Pedido x)
    {
        q.Parameters.Add("@ClienteID", SqlDbType.Int).Value = Db(x.ClienteID);
        q.Parameters.Add("@EmpleadoID", SqlDbType.Int).Value = Db(x.EmpleadoID);
        q.Parameters.Add("@FechaPedido", SqlDbType.Date).Value = x.FechaPedido.Date;
        q.Parameters.Add("@FechaRequerida", SqlDbType.Date).Value = Db(x.FechaRequerida);
        q.Parameters.Add("@FechaEnvio", SqlDbType.Date).Value = Db(x.FechaEnvio);
        q.Parameters.Add("@TransportistaID", SqlDbType.Int).Value = Db(x.TransportistaID);
        q.Parameters.Add("@Destinatario", SqlDbType.NVarChar, 60).Value = Db(x.Destinatario);
        q.Parameters.Add("@CiudadDestino", SqlDbType.NVarChar, 30).Value = Db(x.CiudadDestino);
        q.Parameters.Add("@PaisDestino", SqlDbType.NVarChar, 30).Value = Db(x.PaisDestino);
    }

    private static Pedido Map(DataRow r) => new()
    {
        PedidoID = Convert.ToInt32(r["PedidoID"]),
        ClienteID = r["ClienteID"] == DBNull.Value ? null : Convert.ToInt32(r["ClienteID"]),
        EmpleadoID = r["EmpleadoID"] == DBNull.Value ? null : Convert.ToInt32(r["EmpleadoID"]),
        FechaPedido = Convert.ToDateTime(r["FechaPedido"]),
        FechaRequerida = r["FechaRequerida"] == DBNull.Value ? null : Convert.ToDateTime(r["FechaRequerida"]),
        FechaEnvio = r["FechaEnvio"] == DBNull.Value ? null : Convert.ToDateTime(r["FechaEnvio"]),
        TransportistaID = r["TransportistaID"] == DBNull.Value ? null : Convert.ToInt32(r["TransportistaID"]),
        Destinatario = r["Destinatario"] == DBNull.Value ? null : Convert.ToString(r["Destinatario"]),
        CiudadDestino = r["CiudadDestino"] == DBNull.Value ? null : Convert.ToString(r["CiudadDestino"]),
        PaisDestino = r["PaisDestino"] == DBNull.Value ? null : Convert.ToString(r["PaisDestino"]),
        NombreCliente = r["NombreCliente"] == DBNull.Value ? null : Convert.ToString(r["NombreCliente"]),
        NombreEmpleado = r["NombreEmpleado"] == DBNull.Value ? null : Convert.ToString(r["NombreEmpleado"]),
        NombreTransportista = r["NombreTransportista"] == DBNull.Value ? null : Convert.ToString(r["NombreTransportista"]),
        Total = Convert.ToDecimal(r["Total"])
    };
}
