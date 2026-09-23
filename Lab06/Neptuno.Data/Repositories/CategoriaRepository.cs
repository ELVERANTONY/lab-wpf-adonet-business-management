using System.Data;
using Microsoft.Data.SqlClient;
using Neptuno.Entities;
using static Neptuno.Data.RepositoryHelpers;
namespace Neptuno.Data;
public sealed class CategoriaRepository(string cs) : ICategoriaRepository
{
    public async Task<List<Categoria>> ListarAsync() {
        return await Task.Run(() => {
            using var c = new SqlConnection(cs);
            using var q = Procedure("dbo.usp_Categoria_Listar", c);
            using var da = new SqlDataAdapter(q);
            var dt = new DataTable();
            da.Fill(dt);
            var x = new List<Categoria>();
            foreach(DataRow r in dt.Rows) x.Add(new(){CategoriaID=Convert.ToInt32(r["CategoriaID"]),NombreCategoria=Convert.ToString(r["NombreCategoria"]),Descripcion=r["Descripcion"]==DBNull.Value?null:Convert.ToString(r["Descripcion"])});
            return x;
        });
    }
    public async Task<int> CrearAsync(Categoria x) { await using var c=new SqlConnection(cs); await using var q=Procedure("dbo.usp_Categoria_Crear",c); Add(q,x); await c.OpenAsync(); return Convert.ToInt32(await q.ExecuteScalarAsync()); }
    public async Task ActualizarAsync(Categoria x) { await using var c=new SqlConnection(cs); await using var q=Procedure("dbo.usp_Categoria_Actualizar",c); q.Parameters.Add("@CategoriaID",SqlDbType.Int).Value=x.CategoriaID; Add(q,x); await c.OpenAsync(); await q.ExecuteNonQueryAsync(); }
    public async Task EliminarAsync(int id) { await using var c=new SqlConnection(cs); await using var q=Procedure("dbo.usp_Categoria_Eliminar",c); q.Parameters.Add("@CategoriaID",SqlDbType.Int).Value=id; await c.OpenAsync(); await q.ExecuteNonQueryAsync(); }
    private static void Add(SqlCommand q,Categoria x){q.Parameters.Add("@NombreCategoria",SqlDbType.NVarChar,30).Value=x.NombreCategoria.Trim();q.Parameters.Add("@Descripcion",SqlDbType.NVarChar,200).Value=Db(x.Descripcion);}
}

