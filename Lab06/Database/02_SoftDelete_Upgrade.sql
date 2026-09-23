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
           c.NombreCategoria, pr.CompaniaNombre AS NombreProveedor
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
    SELECT ProveedorID, CompaniaNombre AS NombreCompania, NombreContacto, CargoContacto, 
           Direccion, Ciudad, CodigoPostal AS CodPostal, Pais, Telefono, Fax
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
    @NombreContacto NVARCHAR(40) = NULL,
    @Ciudad NVARCHAR(30) = NULL
AS
BEGIN
    SELECT ProveedorID, CompaniaNombre AS NombreCompania, NombreContacto, CargoContacto, 
           Direccion, Ciudad, CodigoPostal AS CodPostal, Pais, Telefono, Fax
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
    SELECT p.PedidoID, p.ClienteID, p.EmpleadoID, p.FechaPedido, p.FechaRequerida, p.FechaEnvio, 
           p.TransportistaID, 0 AS Cargo, p.Destinatario, p.CiudadDestino, p.PaisDestino,
           c.Empresa AS NombreCliente, e.Apellidos + ', ' + e.Nombre AS NombreEmpleado,
           t.CompaniaNombre AS NombreTransportista,
           COALESCE((SELECT SUM(PrecioUnidad * Cantidad * (1 - Descuento)) FROM DetallePedidos WHERE PedidoID = p.PedidoID), 0) AS Total
    FROM Pedidos p
    LEFT JOIN Clientes c ON p.ClienteID = c.ClienteID
    LEFT JOIN Empleados e ON p.EmpleadoID = e.EmpleadoID
    LEFT JOIN Transportistas t ON p.TransportistaID = t.TransportistaID
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
CREATE OR ALTER PROCEDURE dbo.usp_DetallePedido_ListarPorRangoFechas
    @FechaInicio DATETIME,
    @FechaFin DATETIME
AS
BEGIN
    SELECT p.PedidoID, p.FechaPedido, c.Empresa AS NombreCliente, 
           pr.NombreProducto, d.PrecioUnidad, d.Cantidad, d.Descuento,
           CAST((d.PrecioUnidad * d.Cantidad * (1 - d.Descuento)) AS DECIMAL(18,2)) AS Subtotal
    FROM Pedidos p
    INNER JOIN Clientes c ON p.ClienteID = c.ClienteID
    INNER JOIN DetallePedidos d ON p.PedidoID = d.PedidoID
    INNER JOIN Productos pr ON d.ProductoID = pr.ProductoID
    WHERE p.Activo = 1
      AND p.FechaPedido BETWEEN @FechaInicio AND @FechaFin;
END
GO
