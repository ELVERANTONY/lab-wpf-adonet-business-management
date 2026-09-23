using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Neptuno.Data;
using Neptuno.Entities;
namespace Neptuno.WPF.ViewModels;
public partial class CategoriasViewModel(ICategoriaRepository repository):ObservableObject
{
 public ObservableCollection<Categoria> Categorias{get;}=[];
 [ObservableProperty]private Categoria? seleccionada; [ObservableProperty]private string nombre=string.Empty; [ObservableProperty]private string? descripcion;
 [ObservableProperty]private string? mensaje; [ObservableProperty]private bool esError; [ObservableProperty]private bool estaCargando;
 public Func<string,Task<bool>>? Confirmar{get;set;}
 [RelayCommand]public async Task CargarAsync(){EstaCargando=true;try{var a=await repository.ListarAsync();Categorias.Clear();foreach(var x in a)Categorias.Add(x);Mensaje=Categorias.Count==0?"No existen categorías registradas.":null;}catch(Exception e){Fail(e);}finally{EstaCargando=false;}}
 [RelayCommand]private void Nuevo(){Seleccionada=null;Nombre="";Descripcion=null;Mensaje=null;}
 [RelayCommand]private void Editar(Categoria? x){if(x is null)return;Seleccionada=x;Nombre=x.NombreCategoria;Descripcion=x.Descripcion;Mensaje=null;}
 [RelayCommand]private async Task GuardarAsync(){if(string.IsNullOrWhiteSpace(Nombre)){Mensaje="El nombre de categoría es obligatorio.";EsError=true;return;}EstaCargando=true;try{var edit=Seleccionada is not null;var x=new Categoria{CategoriaID=Seleccionada?.CategoriaID??0,NombreCategoria=Nombre.Trim(),Descripcion=Descripcion};if(edit)await repository.ActualizarAsync(x);else await repository.CrearAsync(x);await CargarAsync();Nuevo();Mensaje=edit?"Categoría actualizada.":"Categoría registrada.";EsError=false;}catch(Exception e){Fail(e);}finally{EstaCargando=false;}}
 [RelayCommand]private async Task EliminarAsync(Categoria? x){if(x is null)return;if(Confirmar is not null&&!await Confirmar($"¿Eliminar la categoría “{x.NombreCategoria}”?"))return;try{await repository.EliminarAsync(x.CategoriaID);await CargarAsync();Mensaje="Categoría eliminada.";EsError=false;}catch(Exception e){Fail(e);}}
 private void Fail(Exception e){Mensaje=$"No se pudo completar la operación: {e.Message}";EsError=true;}
}

