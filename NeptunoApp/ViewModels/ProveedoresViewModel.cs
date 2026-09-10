using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NeptunoApp.Data;
using NeptunoApp.Models;
namespace NeptunoApp.ViewModels;
public partial class ProveedoresViewModel(IProveedorRepository repository):ObservableObject
{
 public ObservableCollection<Proveedor> Proveedores{get;}=[];
 [ObservableProperty]private Proveedor? seleccionado;[ObservableProperty]private string filtroContacto="";[ObservableProperty]private string filtroCiudad="";
 [ObservableProperty]private string compania="";[ObservableProperty]private string? contacto;[ObservableProperty]private string? cargo;[ObservableProperty]private string? direccion;[ObservableProperty]private string? ciudad;[ObservableProperty]private string? codigoPostal;[ObservableProperty]private string? pais;[ObservableProperty]private string? telefono;[ObservableProperty]private string? fax;
 [ObservableProperty]private string? mensaje;[ObservableProperty]private bool esError;[ObservableProperty]private bool estaCargando;
 public Func<string,Task<bool>>? Confirmar{get;set;}
 [RelayCommand]public async Task CargarAsync(){await Load(null,null);}
 [RelayCommand]private async Task BuscarAsync(){await Load(FiltroContacto,FiltroCiudad);if(Proveedores.Count==0){Mensaje="No se encontraron proveedores con esos filtros.";EsError=false;}}
 [RelayCommand]private async Task LimpiarBusquedaAsync(){FiltroContacto="";FiltroCiudad="";await Load(null,null);}
 private async Task Load(string? c,string? city){EstaCargando=true;Mensaje=null;try{var a=await repository.ListarAsync(c,city);Proveedores.Clear();foreach(var x in a)Proveedores.Add(x);}catch(Exception e){Fail(e);}finally{EstaCargando=false;}}
 [RelayCommand]private void Nuevo(){Seleccionado=null;Compania="";Contacto=Cargo=Direccion=Ciudad=CodigoPostal=Pais=Telefono=Fax=null;Mensaje=null;}
 [RelayCommand]private void Editar(Proveedor? x){if(x is null)return;Seleccionado=x;Compania=x.CompaniaNombre;Contacto=x.NombreContacto;Cargo=x.CargoContacto;Direccion=x.Direccion;Ciudad=x.Ciudad;CodigoPostal=x.CodigoPostal;Pais=x.Pais;Telefono=x.Telefono;Fax=x.Fax;Mensaje=null;}
 [RelayCommand]private async Task GuardarAsync(){if(string.IsNullOrWhiteSpace(Compania)){Mensaje="El nombre de la compañía es obligatorio.";EsError=true;return;}EstaCargando=true;try{var edit=Seleccionado is not null;var x=new Proveedor{ProveedorID=Seleccionado?.ProveedorID??0,CompaniaNombre=Compania.Trim(),NombreContacto=Contacto,CargoContacto=Cargo,Direccion=Direccion,Ciudad=Ciudad,CodigoPostal=CodigoPostal,Pais=Pais,Telefono=Telefono,Fax=Fax};if(edit)await repository.ActualizarAsync(x);else await repository.CrearAsync(x);await Load(null,null);Nuevo();Mensaje=edit?"Proveedor actualizado.":"Proveedor registrado.";EsError=false;}catch(Exception e){Fail(e);}finally{EstaCargando=false;}}
 [RelayCommand]private async Task EliminarAsync(Proveedor? x){if(x is null)return;if(Confirmar is not null&&!await Confirmar($"¿Eliminar el proveedor “{x.CompaniaNombre}”?"))return;try{await repository.EliminarAsync(x.ProveedorID);await Load(null,null);Mensaje="Proveedor eliminado.";EsError=false;}catch(Exception e){Fail(e);}}
 private void Fail(Exception e){Mensaje=$"No se pudo completar la operación: {e.Message}";EsError=true;}
}

