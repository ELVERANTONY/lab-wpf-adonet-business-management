namespace NeptunoApp.Models;
public sealed class LineaReporte
{
    public int PedidoID { get; set; }
    public DateTime FechaPedido { get; set; }
    public string? NombreCliente { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public decimal PrecioUnidad { get; set; }
    public short Cantidad { get; set; }
    public decimal Descuento { get; set; }
    public decimal Subtotal { get; set; }
}

