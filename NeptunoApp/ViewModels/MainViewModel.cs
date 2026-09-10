using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace NeptunoApp.ViewModels;
public partial class MainViewModel(ProductosViewModel productos,CategoriasViewModel categorias,ProveedoresViewModel proveedores,PedidosViewModel pedidos,ReportesViewModel reportes) : ObservableObject
{
 public ProductosViewModel Productos{get;}=productos;public CategoriasViewModel Categorias{get;}=categorias;public ProveedoresViewModel Proveedores{get;}=proveedores;public PedidosViewModel Pedidos{get;}=pedidos;public ReportesViewModel Reportes{get;}=reportes;
 [ObservableProperty] private string paginaActual="Productos";
 public bool MostrarProductos=>PaginaActual=="Productos";
 public bool MostrarCategorias=>PaginaActual=="Categorias";
 public bool MostrarProveedores=>PaginaActual=="Proveedores";
 public bool MostrarPedidos=>PaginaActual=="Pedidos";
 public bool MostrarReportes=>PaginaActual=="Reportes";
 partial void OnPaginaActualChanged(string value){OnPropertyChanged(nameof(MostrarProductos));OnPropertyChanged(nameof(MostrarCategorias));OnPropertyChanged(nameof(MostrarProveedores));OnPropertyChanged(nameof(MostrarPedidos));OnPropertyChanged(nameof(MostrarReportes));}
 [RelayCommand] private void VerProductos()=>PaginaActual="Productos";
 [RelayCommand] private void VerCategorias()=>PaginaActual="Categorias";
 [RelayCommand] private void VerProveedores()=>PaginaActual="Proveedores";
 [RelayCommand] private void VerPedidos()=>PaginaActual="Pedidos";
 [RelayCommand] private void VerReportes()=>PaginaActual="Reportes";
}
