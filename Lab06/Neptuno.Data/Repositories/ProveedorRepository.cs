using System.Data;
using Microsoft.Data.SqlClient;
using Neptuno.Entities;
using static Neptuno.Data.RepositoryHelpers;

namespace Neptuno.Data;

public sealed class ProveedorRepository(string cs) : IProveedorRepository
{
    public async Task<List<Proveedor>> ListarAsync(string? contacto = null, string? ciudad = null)
    {
        return await Task.Run(() =>
        {
            using var c = new SqlConnection(cs);
            using var q = Procedure(string.IsNullOrWhiteSpace(contacto) && string.IsNullOrWhiteSpace(ciudad) ? "dbo.usp_Proveedor_Listar" : "dbo.usp_Proveedor_Buscar", c);
            
            if (q.CommandText.EndsWith("Buscar"))
            {
                q.Parameters.Add("@NombreContacto", SqlDbType.NVarChar, 40).Value = Db(contacto);
                q.Parameters.Add("@Ciudad", SqlDbType.NVarChar, 30).Value = Db(ciudad);
            }

            using var da = new SqlDataAdapter(q);
            var dt = new DataTable();
            da.Fill(dt); // Fills completely disconnected

            var a = new List<Proveedor>();
            foreach (DataRow r in dt.Rows)
                a.Add(Map(r));

            return a;
        });
    }

    public async Task<int> CrearAsync(Proveedor x)
    {
        await using var c = new SqlConnection(cs);
        await using var q = Procedure("dbo.usp_Proveedor_Crear", c);
        Add(q, x);
        await c.OpenAsync();
        return Convert.ToInt32(await q.ExecuteScalarAsync());
    }

    public async Task ActualizarAsync(Proveedor x)
    {
        await using var c = new SqlConnection(cs);
        await using var q = Procedure("dbo.usp_Proveedor_Actualizar", c);
        q.Parameters.Add("@ProveedorID", SqlDbType.Int).Value = x.ProveedorID;
        Add(q, x);
        await c.OpenAsync();
        await q.ExecuteNonQueryAsync();
    }

    public async Task EliminarAsync(int id)
    {
        await using var c = new SqlConnection(cs);
        await using var q = Procedure("dbo.usp_Proveedor_Eliminar", c);
        q.Parameters.Add("@ProveedorID", SqlDbType.Int).Value = id;
        await c.OpenAsync();
        await q.ExecuteNonQueryAsync();
    }

    private static void Add(SqlCommand q, Proveedor x)
    {
        q.Parameters.Add("@CompaniaNombre", SqlDbType.NVarChar, 60).Value = x.CompaniaNombre.Trim();
        q.Parameters.Add("@NombreContacto", SqlDbType.NVarChar, 40).Value = Db(x.NombreContacto);
        q.Parameters.Add("@CargoContacto", SqlDbType.NVarChar, 40).Value = Db(x.CargoContacto);
        q.Parameters.Add("@Direccion", SqlDbType.NVarChar, 80).Value = Db(x.Direccion);
        q.Parameters.Add("@Ciudad", SqlDbType.NVarChar, 30).Value = Db(x.Ciudad);
        q.Parameters.Add("@CodigoPostal", SqlDbType.NVarChar, 10).Value = Db(x.CodigoPostal);
        q.Parameters.Add("@Pais", SqlDbType.NVarChar, 30).Value = Db(x.Pais);
        q.Parameters.Add("@Telefono", SqlDbType.NVarChar, 24).Value = Db(x.Telefono);
        q.Parameters.Add("@Fax", SqlDbType.NVarChar, 24).Value = Db(x.Fax);
    }

    private static Proveedor Map(DataRow r) => new()
    {
        ProveedorID = Convert.ToInt32(r["ProveedorID"]),
        CompaniaNombre = Convert.ToString(r["NombreCompania"]),
        NombreContacto = r["NombreContacto"] == DBNull.Value ? null : Convert.ToString(r["NombreContacto"]),
        CargoContacto = r["CargoContacto"] == DBNull.Value ? null : Convert.ToString(r["CargoContacto"]),
        Direccion = r["Direccion"] == DBNull.Value ? null : Convert.ToString(r["Direccion"]),
        Ciudad = r["Ciudad"] == DBNull.Value ? null : Convert.ToString(r["Ciudad"]),
        CodigoPostal = r["CodPostal"] == DBNull.Value ? null : Convert.ToString(r["CodPostal"]),
        Pais = r["Pais"] == DBNull.Value ? null : Convert.ToString(r["Pais"]),
        Telefono = r["Telefono"] == DBNull.Value ? null : Convert.ToString(r["Telefono"]),
        Fax = r["Fax"] == DBNull.Value ? null : Convert.ToString(r["Fax"])
    };
}
