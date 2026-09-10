using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;
using static NeptunoApp.Data.RepositoryHelpers;
namespace NeptunoApp.Data;
public sealed class CategoriaRepository(string cs) : ICategoriaRepository
{
    public async Task<List<Categoria>> ListarAsync() { await using var c=new SqlConnection(cs); await using var q=Procedure("dbo.usp_Categoria_Listar",c); await c.OpenAsync(); await using var r=await q.ExecuteReaderAsync(); var x=new List<Categoria>(); while(await r.ReadAsync()) x.Add(new(){CategoriaID=r.GetInt32(0),NombreCategoria=r.GetString(1),Descripcion=Text(r,"Descripcion")}); return x; }
    public async Task<int> CrearAsync(Categoria x) { await using var c=new SqlConnection(cs); await using var q=Procedure("dbo.usp_Categoria_Crear",c); Add(q,x); await c.OpenAsync(); return Convert.ToInt32(await q.ExecuteScalarAsync()); }
    public async Task ActualizarAsync(Categoria x) { await using var c=new SqlConnection(cs); await using var q=Procedure("dbo.usp_Categoria_Actualizar",c); q.Parameters.Add("@CategoriaID",SqlDbType.Int).Value=x.CategoriaID; Add(q,x); await c.OpenAsync(); await q.ExecuteNonQueryAsync(); }
    public async Task EliminarAsync(int id) { await using var c=new SqlConnection(cs); await using var q=Procedure("dbo.usp_Categoria_Eliminar",c); q.Parameters.Add("@CategoriaID",SqlDbType.Int).Value=id; await c.OpenAsync(); await q.ExecuteNonQueryAsync(); }
    private static void Add(SqlCommand q,Categoria x){q.Parameters.Add("@NombreCategoria",SqlDbType.NVarChar,30).Value=x.NombreCategoria.Trim();q.Parameters.Add("@Descripcion",SqlDbType.NVarChar,200).Value=Db(x.Descripcion);}
}

