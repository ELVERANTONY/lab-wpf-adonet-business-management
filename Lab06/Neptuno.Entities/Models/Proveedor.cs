namespace Neptuno.Entities;
public sealed class Proveedor
{
    public int ProveedorID { get; set; }
    public string CompaniaNombre { get; set; } = string.Empty;
    public string? NombreContacto { get; set; }
    public string? CargoContacto { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public string? CodigoPostal { get; set; }
    public string? Pais { get; set; }
    public string? Telefono { get; set; }
    public string? Fax { get; set; }
    public Proveedor() { }
    public Proveedor(int id, string nombre) { ProveedorID = id; CompaniaNombre = nombre; }
}
