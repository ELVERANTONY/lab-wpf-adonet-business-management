using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NeptunoApp.Data;
using NeptunoApp.Models;

namespace NeptunoApp.ViewModels;

public partial class ProductosViewModel : ObservableObject
{
    private readonly IProductoRepository repository;
    public ObservableCollection<Producto> Productos { get; } = [];
    public ObservableCollection<Categoria> Categorias { get; } = [];
    public ObservableCollection<Proveedor> Proveedores { get; } = [];

    [ObservableProperty] private Producto? productoSeleccionado;
    [ObservableProperty] private string nombreProducto = string.Empty;
    [ObservableProperty] private Categoria? categoriaSeleccionada;
    [ObservableProperty] private Proveedor? proveedorSeleccionado;
    [ObservableProperty] private string? cantidadPorUnidad;
    [ObservableProperty] private decimal precioUnidad;
    [ObservableProperty] private decimal unidadesEnExistencia;
    [ObservableProperty] private decimal unidadesEnPedido;
    [ObservableProperty] private decimal nivelDeReorden;
    [ObservableProperty] private bool descontinuado;
    [ObservableProperty] private string? mensajeError;
    [ObservableProperty] private string? mensajeExito;
    [ObservableProperty] private bool estaCargando;
    [ObservableProperty] private bool estaEditando;
    public bool TieneProductos => Productos.Count > 0;
    public Func<Producto, Task<bool>>? ConfirmarEliminacion { get; set; }

    public ProductosViewModel(IProductoRepository repository) => this.repository = repository;

    [RelayCommand]
    private async Task CargarAsync()
    {
        EstaCargando = true;
        LimpiarMensajes();
        try
        {
            var productos = await repository.ListarAsync();
            var categorias = await repository.ListarCategoriasAsync();
            var proveedores = await repository.ListarProveedoresAsync();
            Productos.Clear(); foreach (var item in productos) Productos.Add(item);
            Categorias.Clear(); foreach (var item in categorias) Categorias.Add(item);
            Proveedores.Clear(); foreach (var item in proveedores) Proveedores.Add(item);
            OnPropertyChanged(nameof(TieneProductos));
        }
        catch (Exception ex) { MensajeError = Friendly(ex, "cargar los productos"); }
        finally { EstaCargando = false; }
    }

    [RelayCommand]
    private void Nuevo()
    {
        ProductoSeleccionado = null;
        NombreProducto = string.Empty; CategoriaSeleccionada = null; ProveedorSeleccionado = null;
        CantidadPorUnidad = null; PrecioUnidad = 0; UnidadesEnExistencia = 0;
        UnidadesEnPedido = 0; NivelDeReorden = 0; Descontinuado = false;
        EstaEditando = false; LimpiarMensajes();
    }

    [RelayCommand]
    private void Editar(Producto? producto)
    {
        if (producto is null) return;
        ProductoSeleccionado = producto;
        NombreProducto = producto.NombreProducto;
        CategoriaSeleccionada = Categorias.FirstOrDefault(x => x.CategoriaID == producto.CategoriaID);
        ProveedorSeleccionado = Proveedores.FirstOrDefault(x => x.ProveedorID == producto.ProveedorID);
        CantidadPorUnidad = producto.CantidadPorUnidad;
        PrecioUnidad = producto.PrecioUnidad; UnidadesEnExistencia = producto.UnidadesEnExistencia;
        UnidadesEnPedido = producto.UnidadesEnPedido; NivelDeReorden = producto.NivelDeReorden;
        Descontinuado = producto.Descontinuado; EstaEditando = true; LimpiarMensajes();
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (!Validar()) return;
        EstaCargando = true; LimpiarMensajes();
        try
        {
            var eraEdicion = EstaEditando;
            var producto = CrearDesdeFormulario();
            if (eraEdicion) { await repository.ActualizarAsync(producto); MensajeExito = "Producto actualizado correctamente."; }
            else { await repository.CrearAsync(producto); MensajeExito = "Producto registrado correctamente."; }
            await RecargarSinBorrarMensajeAsync();
            Nuevo(); MensajeExito = eraEdicion ? "Producto actualizado correctamente." : "Producto registrado correctamente.";
        }
        catch (Exception ex) { MensajeError = Friendly(ex, "guardar el producto"); }
        finally { EstaCargando = false; }
    }

    [RelayCommand]
    private async Task EliminarAsync(Producto? producto)
    {
        if (producto is null) return;
        if (ConfirmarEliminacion is not null && !await ConfirmarEliminacion(producto)) return;
        EstaCargando = true; LimpiarMensajes();
        try
        {
            await repository.EliminarAsync(producto.ProductoID);
            Productos.Remove(producto); OnPropertyChanged(nameof(TieneProductos));
            if (ProductoSeleccionado?.ProductoID == producto.ProductoID) Nuevo();
            MensajeExito = "Producto eliminado correctamente.";
        }
        catch (Exception ex) { MensajeError = Friendly(ex, "eliminar el producto"); }
        finally { EstaCargando = false; }
    }

    private async Task RecargarSinBorrarMensajeAsync()
    {
        var productos = await repository.ListarAsync();
        Productos.Clear(); foreach (var item in productos) Productos.Add(item);
        OnPropertyChanged(nameof(TieneProductos));
    }

    private bool Validar()
    {
        LimpiarMensajes();
        if (string.IsNullOrWhiteSpace(NombreProducto)) { MensajeError = "El nombre del producto es obligatorio."; return false; }
        if (NombreProducto.Trim().Length > 60) { MensajeError = "El nombre admite como máximo 60 caracteres."; return false; }
        if (CategoriaSeleccionada is null || ProveedorSeleccionado is null) { MensajeError = "Selecciona una categoría y un proveedor."; return false; }
        if (PrecioUnidad < 0 || UnidadesEnExistencia < 0 || UnidadesEnPedido < 0 || NivelDeReorden < 0)
        { MensajeError = "Los valores numéricos no pueden ser negativos."; return false; }
        if (UnidadesEnExistencia > short.MaxValue || UnidadesEnPedido > short.MaxValue || NivelDeReorden > short.MaxValue)
        { MensajeError = $"Las unidades no pueden superar {short.MaxValue}."; return false; }
        return true;
    }

    private Producto CrearDesdeFormulario() => new()
    {
        ProductoID = ProductoSeleccionado?.ProductoID ?? 0,
        NombreProducto = NombreProducto.Trim(), CategoriaID = CategoriaSeleccionada?.CategoriaID,
        ProveedorID = ProveedorSeleccionado?.ProveedorID, CantidadPorUnidad = CantidadPorUnidad?.Trim(),
        PrecioUnidad = PrecioUnidad, UnidadesEnExistencia = (short)UnidadesEnExistencia,
        UnidadesEnPedido = (short)UnidadesEnPedido, NivelDeReorden = (short)NivelDeReorden,
        Descontinuado = Descontinuado
    };

    private void LimpiarMensajes() { MensajeError = null; MensajeExito = null; }
    private static string Friendly(Exception ex, string action) =>
        ex is InvalidOperationException && ex.Message.Contains("MSSQL_")
            ? ex.Message
            : $"No se pudo {action}. Verifica Docker y la conexión. Detalle: {ex.Message}";
}
