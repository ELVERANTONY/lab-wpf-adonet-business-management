using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NeptunoApp.Data;
using NeptunoApp.Models;
namespace NeptunoApp.ViewModels;
public partial class ReportesViewModel(IPedidoRepository repository):ObservableObject
{
 public ObservableCollection<LineaReporte> Lineas{get;}=[];
 [ObservableProperty]private DateTimeOffset? fechaInicio=new(new DateTime(2026,8,1));[ObservableProperty]private DateTimeOffset? fechaFin=new(DateTime.Today);
 [ObservableProperty]private string? mensaje;[ObservableProperty]private bool esError;[ObservableProperty]private bool estaCargando;[ObservableProperty]private decimal total;
 [RelayCommand]public async Task ConsultarAsync(){Lineas.Clear();Total=0;if(FechaInicio is null||FechaFin is null){Mensaje="Selecciona ambas fechas.";EsError=true;return;}if(FechaInicio>FechaFin){Mensaje="La fecha inicial no puede ser posterior a la fecha final.";EsError=true;return;}EstaCargando=true;Mensaje=null;try{var a=await repository.ReportarAsync(FechaInicio.Value.Date,FechaFin.Value.Date);foreach(var x in a)Lineas.Add(x);Total=Lineas.Sum(x=>x.Subtotal);Mensaje=Lineas.Count==0?"No existen detalles de pedidos en el intervalo.":$"{Lineas.Count} detalle(s) encontrado(s).";EsError=false;}catch(Exception e){Mensaje=$"No se pudo consultar el reporte: {e.Message}";EsError=true;}finally{EstaCargando=false;}}
}
