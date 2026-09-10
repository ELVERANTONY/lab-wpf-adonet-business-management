using Avalonia.Controls;
using Avalonia.Interactivity;
using NeptunoApp.ViewModels;

namespace NeptunoApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Opened += OnOpened;
        DataContextChanged += (_, _) => AttachConfirmations();
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        AttachConfirmations();
        if (DataContext is MainViewModel vm)
        {
            await vm.Productos.CargarCommand.ExecuteAsync(null);
            await vm.Categorias.CargarCommand.ExecuteAsync(null);
            await vm.Proveedores.CargarCommand.ExecuteAsync(null);
            await vm.Pedidos.CargarCommand.ExecuteAsync(null);
            await vm.Reportes.ConsultarCommand.ExecuteAsync(null);
        }
    }

    private void AttachConfirmations()
    {
        if (DataContext is not MainViewModel vm) return;
        vm.Productos.ConfirmarEliminacion = p => ConfirmAsync($"¿Eliminar el producto “{p.NombreProducto}”?");
        vm.Categorias.Confirmar = ConfirmAsync;
        vm.Proveedores.Confirmar = ConfirmAsync;
        vm.Pedidos.Confirmar = ConfirmAsync;
    }

    private async Task<bool> ConfirmAsync(string question)
    {
        var dialog = new Window
        {
            Title = "Confirmar eliminación", Width = 430, Height = 190,
            CanResize = false, WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        var result = false;
        var yes = new Button { Content = "Eliminar", Classes = { "danger" } };
        var no = new Button { Content = "Cancelar" };
        yes.Click += (_, _) => { result = true; dialog.Close(); };
        no.Click += (_, _) => dialog.Close();
        dialog.Content = new StackPanel
        {
            Margin = new Avalonia.Thickness(24), Spacing = 18,
            Children =
            {
                new TextBlock { Text = question, FontSize = 17, TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                new TextBlock { Text = "Esta acción no se puede deshacer.", Foreground = Avalonia.Media.Brushes.DimGray },
                new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal, Spacing = 10, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right, Children = { no, yes } }
            }
        };
        await dialog.ShowDialog(this);
        return result;
    }
}
