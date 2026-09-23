using Neptuno.Entities;
namespace Neptuno.Data;
public interface IProveedorRepository
{
 Task<List<Proveedor>> ListarAsync(string? contacto=null,string? ciudad=null); Task<int> CrearAsync(Proveedor item);
 Task ActualizarAsync(Proveedor item); Task EliminarAsync(int id);
}

