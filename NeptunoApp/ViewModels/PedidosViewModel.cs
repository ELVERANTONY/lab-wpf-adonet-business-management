using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NeptunoApp.Data;
using NeptunoApp.Models;
namespace NeptunoApp.ViewModels;
public partial class PedidosViewModel(IPedidoRepository repository):ObservableObject
{
 public ObservableCollection<Pedido> Pedidos{get;}=[];public ObservableCollection<Cliente> Clientes{get;}=[];public ObservableCollection<Empleado> Empleados{get;}=[];public ObservableCollection<Transportista> Transportistas{get;}=[];
 [ObservableProperty]private Pedido? seleccionado;[ObservableProperty]private Cliente? cliente;[ObservableProperty]private Empleado? empleado;[ObservableProperty]private Transportista? transportista;
 [ObservableProperty]private DateTimeOffset? fechaPedido=new(DateTime.Today);[ObservableProperty]private DateTimeOffset? fechaRequerida;[ObservableProperty]private DateTimeOffset? fechaEnvio;
 [ObservableProperty]private string? destinatario;[ObservableProperty]private string? ciudadDestino;[ObservableProperty]private string? paisDestino;
 [ObservableProperty]private string? mensaje;[ObservableProperty]private bool esError;[ObservableProperty]private bool estaCargando;public Func<string,Task<bool>>? Confirmar{get;set;}
 [RelayCommand]public async Task CargarAsync(){EstaCargando=true;try{var p=await repository.ListarAsync();var c=await repository.ListarClientesAsync();var e=await repository.ListarEmpleadosAsync();var t=await repository.ListarTransportistasAsync();Pedidos.Clear();foreach(var x in p)Pedidos.Add(x);Clientes.Clear();foreach(var x in c)Clientes.Add(x);Empleados.Clear();foreach(var x in e)Empleados.Add(x);Transportistas.Clear();foreach(var x in t)Transportistas.Add(x);}catch(Exception e){Fail(e);}finally{EstaCargando=false;}}
 [RelayCommand]private void Nuevo(){Seleccionado=null;Cliente=null;Empleado=null;Transportista=null;FechaPedido=new(DateTime.Today);FechaRequerida=null;FechaEnvio=null;Destinatario=CiudadDestino=PaisDestino=null;Mensaje=null;}
 [RelayCommand]private void Editar(Pedido? x){if(x is null)return;Seleccionado=x;Cliente=Clientes.FirstOrDefault(c=>c.ClienteID==x.ClienteID);Empleado=Empleados.FirstOrDefault(e=>e.EmpleadoID==x.EmpleadoID);Transportista=Transportistas.FirstOrDefault(t=>t.TransportistaID==x.TransportistaID);FechaPedido=new(x.FechaPedido);FechaRequerida=x.FechaRequerida is null?null:new(x.FechaRequerida.Value);FechaEnvio=x.FechaEnvio is null?null:new(x.FechaEnvio.Value);Destinatario=x.Destinatario;CiudadDestino=x.CiudadDestino;PaisDestino=x.PaisDestino;Mensaje=null;}
 [RelayCommand]private async Task GuardarAsync(){if(FechaPedido is null){Mensaje="La fecha del pedido es obligatoria.";EsError=true;return;}if(FechaRequerida<FechaPedido){Mensaje="La fecha requerida no puede ser anterior a la fecha del pedido.";EsError=true;return;}EstaCargando=true;try{var edit=Seleccionado is not null;var x=new Pedido{PedidoID=Seleccionado?.PedidoID??0,ClienteID=Cliente?.ClienteID,EmpleadoID=Empleado?.EmpleadoID,TransportistaID=Transportista?.TransportistaID,FechaPedido=FechaPedido.Value.Date,FechaRequerida=FechaRequerida?.Date,FechaEnvio=FechaEnvio?.Date,Destinatario=Destinatario,CiudadDestino=CiudadDestino,PaisDestino=PaisDestino};if(edit)await repository.ActualizarAsync(x);else await repository.CrearAsync(x);await CargarAsync();Nuevo();Mensaje=edit?"Pedido actualizado.":"Pedido registrado.";EsError=false;}catch(Exception e){Fail(e);}finally{EstaCargando=false;}}
 [RelayCommand]private async Task EliminarAsync(Pedido? x){if(x is null)return;if(Confirmar is not null&&!await Confirmar($"¿Eliminar el pedido #{x.PedidoID} y sus detalles?"))return;try{await repository.EliminarAsync(x.PedidoID);await CargarAsync();Mensaje="Pedido eliminado.";EsError=false;}catch(Exception e){Fail(e);}}
 private void Fail(Exception e){Mensaje=$"No se pudo completar la operación: {e.Message}";EsError=true;}
}

