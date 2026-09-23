USE NeptunoDB;
GO

-- 1. Agregar columna Activo a las tablas principales
IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'Activo' AND Object_ID = Object_ID(N'Productos'))
BEGIN
    ALTER TABLE Productos ADD Activo BIT NOT NULL DEFAULT 1;
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'Activo' AND Object_ID = Object_ID(N'Categorias'))
BEGIN
    ALTER TABLE Categorias ADD Activo BIT NOT NULL DEFAULT 1;
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'Activo' AND Object_ID = Object_ID(N'Proveedores'))
BEGIN
    ALTER TABLE Proveedores ADD Activo BIT NOT NULL DEFAULT 1;
END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'Activo' AND Object_ID = Object_ID(N'Pedidos'))
BEGIN
    ALTER TABLE Pedidos ADD Activo BIT NOT NULL DEFAULT 1;
END
GO

-- 2. Modificar Procedimiento: Productos (Soft Delete)
CREATE OR ALTER PROCEDURE dbo.usp_Producto_Listar
AS
BEGIN
    SELECT p.ProductoID, p.NombreProducto, p.ProveedorID, p.CategoriaID, 
           p.CantidadPorUnidad, p.PrecioUnidad, p.UnidadesEnExistencia, 
           p.UnidadesEnPedido, p.NivelDeReorden, p.Descontinuado,
           c.NombreCategoria, pr.NombreCompania AS NombreProveedor
    FROM Productos p
    LEFT JOIN Categorias c ON p.CategoriaID = c.CategoriaID
    LEFT JOIN Proveedores pr ON p.ProveedorID = pr.ProveedorID
    WHERE p.Activo = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Producto_Eliminar
    @ProductoID INT
AS
BEGIN
    UPDATE Productos SET Activo = 0 WHERE ProductoID = @ProductoID;
END
GO

-- 3. Modificar Procedimiento: Categorias (Soft Delete)
CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Listar
AS
BEGIN
    SELECT CategoriaID, NombreCategoria, Descripcion
    FROM Categorias
    WHERE Activo = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Eliminar
    @CategoriaID INT
AS
BEGIN
    UPDATE Categorias SET Activo = 0 WHERE CategoriaID = @CategoriaID;
END
GO

-- 4. Modificar Procedimiento: Proveedores (Soft Delete)
CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Listar
AS
BEGIN
    SELECT ProveedorID, NombreCompania, NombreContacto, CargoContacto, 
           Direccion, Ciudad, Region, CodPostal, Pais, Telefono, Fax
    FROM Proveedores
    WHERE Activo = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Eliminar
    @ProveedorID INT
AS
BEGIN
    UPDATE Proveedores SET Activo = 0 WHERE ProveedorID = @ProveedorID;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Buscar
    @NombreContacto NVARCHAR(30) = NULL,
    @Ciudad NVARCHAR(15) = NULL
AS
BEGIN
    SELECT ProveedorID, NombreCompania, NombreContacto, CargoContacto, 
           Direccion, Ciudad, Region, CodPostal, Pais, Telefono, Fax
    FROM Proveedores
    WHERE Activo = 1
      AND (@NombreContacto IS NULL OR NombreContacto LIKE '%' + @NombreContacto + '%')
      AND (@Ciudad IS NULL OR Ciudad LIKE '%' + @Ciudad + '%');
END
GO

-- 5. Modificar Procedimiento: Pedidos (Soft Delete)
CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Listar
AS
BEGIN
    SELECT p.PedidoID, p.ClienteID, p.EmpleadoID, p.FechaPedido, p.FechaEntrega, p.FechaEnvio, 
           p.FormaEnvio, p.Cargo, p.Destinatario, p.DireccionDestinatario, p.CiudadDestinatario, 
           p.RegionDestinatario, p.CodPostalDestinatario, p.PaisDestinatario,
           c.NombreCompania AS NombreCliente, e.Apellidos + ', ' + e.Nombre AS NombreEmpleado,
           ev.NombreCompania AS NombreCompaniaEnvio
    FROM Pedidos p
    LEFT JOIN Clientes c ON p.ClienteID = c.ClienteID
    LEFT JOIN Empleados e ON p.EmpleadoID = e.EmpleadoID
    LEFT JOIN companiasdeenvios ev ON p.FormaEnvio = ev.IdCompaniaEnvio
    WHERE p.Activo = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Eliminar
    @PedidoID INT
AS
BEGIN
    UPDATE Pedidos SET Activo = 0 WHERE Pedidos.PedidoID = @PedidoID;
END
GO

-- 6. Modificar Procedimiento de Reportes (Filtro Activo=1)
CREATE OR ALTER PROCEDURE dbo.usp_Reporte_PedidosPorFecha
    @FechaInicio DATETIME,
    @FechaFin DATETIME
AS
BEGIN
    SELECT p.PedidoID, p.FechaPedido, c.NombreCompania AS ClienteNombre, 
           pr.NombreProducto, d.PrecioUnidad, d.Cantidad, d.Descuento
    FROM Pedidos p
    INNER JOIN Clientes c ON p.ClienteID = c.ClienteID
    INNER JOIN [Detalles de pedidos] d ON p.PedidoID = d.PedidoID
    INNER JOIN Productos pr ON d.ProductoID = pr.ProductoID
    WHERE p.Activo = 1
      AND p.FechaPedido BETWEEN @FechaInicio AND @FechaFin;
END
GO
