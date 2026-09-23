using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NeptunoApp.Data;

namespace NeptunoApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public ProductosViewModel Productos { get; }
    public CategoriasViewModel Categorias { get; }
    public ProveedoresViewModel Proveedores { get; }
    public PedidosViewModel Pedidos { get; }
    public ReportesViewModel Reportes { get; }

    [ObservableProperty] private object? vistaActual;
    [ObservableProperty] private string paginaActual = "Productos";

    public MainViewModel()
    {
        var cs = DbConfig.GetConnectionString();
        var prodRepo = new ProductoRepository(cs);
        var catRepo = new CategoriaRepository(cs);
        var provRepo = new ProveedorRepository(cs);
        var pedRepo = new PedidoRepository(cs);

        Productos = new ProductosViewModel(prodRepo);
        Categorias = new CategoriasViewModel(catRepo);
        Proveedores = new ProveedoresViewModel(provRepo);
        Pedidos = new PedidosViewModel(pedRepo);
        Reportes = new ReportesViewModel(pedRepo);

        VistaActual = Productos;
        PaginaActual = "Productos";

        // Cargar datos iniciales
        _ = Productos.CargarCommand.ExecuteAsync(null);
        _ = Categorias.CargarCommand.ExecuteAsync(null);
        _ = Proveedores.CargarCommand.ExecuteAsync(null);
        _ = Pedidos.CargarCommand.ExecuteAsync(null);
    }

    public MainViewModel(
        ProductosViewModel productos,
        CategoriasViewModel categorias,
        ProveedoresViewModel proveedores,
        PedidosViewModel pedidos,
        ReportesViewModel reportes)
    {
        Productos = productos;
        Categorias = categorias;
        Proveedores = proveedores;
        Pedidos = pedidos;
        Reportes = reportes;
        VistaActual = productos;
        PaginaActual = "Productos";
    }

    [RelayCommand]
    private void VerProductos()
    {
        VistaActual = Productos;
        PaginaActual = "Productos";
    }

    [RelayCommand]
    private void VerCategorias()
    {
        VistaActual = Categorias;
        PaginaActual = "Categorias";
    }

    [RelayCommand]
    private void VerProveedores()
    {
        VistaActual = Proveedores;
        PaginaActual = "Proveedores";
    }

    [RelayCommand]
    private void VerPedidos()
    {
        VistaActual = Pedidos;
        PaginaActual = "Pedidos";
    }

    [RelayCommand]
    private void VerReportes()
    {
        VistaActual = Reportes;
        PaginaActual = "Reportes";
    }
}
