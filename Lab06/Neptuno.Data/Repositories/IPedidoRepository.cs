using Neptuno.Entities;
namespace Neptuno.Data;
public interface IPedidoRepository
{
 Task<List<Pedido>> ListarAsync(); Task<int> CrearAsync(Pedido item); Task ActualizarAsync(Pedido item); Task EliminarAsync(int id);
 Task<List<Cliente>> ListarClientesAsync(); Task<List<Empleado>> ListarEmpleadosAsync(); Task<List<Transportista>> ListarTransportistasAsync();
 Task<List<LineaReporte>> ReportarAsync(DateTime inicio,DateTime fin);
}

