namespace Neptuno.Entities;
public sealed class Pedido
{
    public int PedidoID { get; set; }
    public int? ClienteID { get; set; }
    public int? EmpleadoID { get; set; }
    public DateTime FechaPedido { get; set; } = DateTime.Today;
    public DateTime? FechaRequerida { get; set; }
    public DateTime? FechaEnvio { get; set; }
    public int? TransportistaID { get; set; }
    public string? Destinatario { get; set; }
    public string? CiudadDestino { get; set; }
    public string? PaisDestino { get; set; }
    public string? NombreCliente { get; set; }
    public string? NombreEmpleado { get; set; }
    public string? NombreTransportista { get; set; }
    public decimal Total { get; set; }
}

public sealed record Cliente(int ClienteID, string Empresa);
public sealed record Empleado(int EmpleadoID, string NombreCompleto);
public sealed record Transportista(int TransportistaID, string CompaniaNombre);

