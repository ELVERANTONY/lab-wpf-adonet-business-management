using Neptuno.Entities;

namespace Neptuno.Data;

public interface IProductoRepository
{
    Task<List<Producto>> ListarAsync();
    Task<int> CrearAsync(Producto producto);
    Task ActualizarAsync(Producto producto);
    Task EliminarAsync(int productoId);
    Task<List<Categoria>> ListarCategoriasAsync();
    Task<List<Proveedor>> ListarProveedoresAsync();
}

