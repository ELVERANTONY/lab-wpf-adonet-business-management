using Neptuno.Entities;
namespace Neptuno.Data;
public interface ICategoriaRepository
{
    Task<List<Categoria>> ListarAsync(); Task<int> CrearAsync(Categoria item);
    Task ActualizarAsync(Categoria item); Task EliminarAsync(int id);
}

