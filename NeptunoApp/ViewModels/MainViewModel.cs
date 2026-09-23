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
    }

    [ObservableProperty] private string paginaActual = "Productos";
    public bool MostrarProductos => PaginaActual == "Productos";
    public bool MostrarCategorias => PaginaActual == "Categorias";
    public bool MostrarProveedores => PaginaActual == "Proveedores";
    public bool MostrarPedidos => PaginaActual == "Pedidos";
    public bool MostrarReportes => PaginaActual == "Reportes";

    partial void OnPaginaActualChanged(string value)
    {
        OnPropertyChanged(nameof(MostrarProductos));
        OnPropertyChanged(nameof(MostrarCategorias));
        OnPropertyChanged(nameof(MostrarProveedores));
        OnPropertyChanged(nameof(MostrarPedidos));
        OnPropertyChanged(nameof(MostrarReportes));
    }

    [RelayCommand] private void VerProductos() => PaginaActual = "Productos";
    [RelayCommand] private void VerCategorias() => PaginaActual = "Categorias";
    [RelayCommand] private void VerProveedores() => PaginaActual = "Proveedores";
    [RelayCommand] private void VerPedidos() => PaginaActual = "Pedidos";
    [RelayCommand] private void VerReportes() => PaginaActual = "Reportes";
}
