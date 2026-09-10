using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using NeptunoApp.Data;
using NeptunoApp.ViewModels;
using NeptunoApp.Views;

namespace NeptunoApp;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var cs = DbConfig.GetConnectionString();
            var productos = new ProductosViewModel(new ProductoRepository(cs));
            var categorias = new CategoriasViewModel(new CategoriaRepository(cs));
            var proveedores = new ProveedoresViewModel(new ProveedorRepository(cs));
            var pedidosRepository = new PedidoRepository(cs);
            var pedidos = new PedidosViewModel(pedidosRepository);
            var reportes = new ReportesViewModel(pedidosRepository);
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(productos, categorias, proveedores, pedidos, reportes)
            };
        }
        base.OnFrameworkInitializationCompleted();
    }
}
