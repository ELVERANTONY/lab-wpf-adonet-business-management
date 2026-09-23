using CommunityToolkit.Mvvm.ComponentModel;

namespace Neptuno.Entities;

public partial class Producto : ObservableObject
{
    public int ProductoID { get; set; }
    [ObservableProperty] private string nombreProducto = string.Empty;
    [ObservableProperty] private int? proveedorID;
    [ObservableProperty] private int? categoriaID;
    [ObservableProperty] private string? cantidadPorUnidad;
    [ObservableProperty] private decimal precioUnidad;
    [ObservableProperty] private short unidadesEnExistencia;
    [ObservableProperty] private short unidadesEnPedido;
    [ObservableProperty] private short nivelDeReorden;
    [ObservableProperty] private bool descontinuado;
    public string? NombreCategoria { get; set; }
    public string? NombreProveedor { get; set; }
}

