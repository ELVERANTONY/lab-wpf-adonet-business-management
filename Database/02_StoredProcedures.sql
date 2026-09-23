USE NeptunoDB;
GO

----------------------------------------------------
-- CATEGORIAS
----------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Listar
AS
BEGIN
    SELECT CategoriaID, NombreCategoria, Descripcion
    FROM Categorias;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Crear
    @NombreCategoria NVARCHAR(30),
    @Descripcion NVARCHAR(200) = NULL
AS
BEGIN
    INSERT INTO Categorias (NombreCategoria, Descripcion)
    VALUES (@NombreCategoria, @Descripcion);
    
    SELECT SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Actualizar
    @CategoriaID INT,
    @NombreCategoria NVARCHAR(30),
    @Descripcion NVARCHAR(200) = NULL
AS
BEGIN
    UPDATE Categorias
    SET NombreCategoria = @NombreCategoria,
        Descripcion = @Descripcion
    WHERE CategoriaID = @CategoriaID;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Eliminar
    @CategoriaID INT
AS
BEGIN
    DELETE FROM Categorias WHERE CategoriaID = @CategoriaID;
END
GO

----------------------------------------------------
-- PROVEEDORES
----------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Listar
AS
BEGIN
    SELECT ProveedorID, CompaniaNombre AS NombreCompania, NombreContacto, CargoContacto, 
           Direccion, Ciudad, CodigoPostal AS CodPostal, Pais, Telefono, Fax
    FROM Proveedores;
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
    WHERE (@NombreContacto IS NULL OR NombreContacto LIKE '%' + @NombreContacto + '%')
      AND (@Ciudad IS NULL OR Ciudad LIKE '%' + @Ciudad + '%');
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Crear
    @CompaniaNombre NVARCHAR(60),
    @NombreContacto NVARCHAR(40) = NULL,
    @CargoContacto NVARCHAR(40) = NULL,
    @Direccion NVARCHAR(80) = NULL,
    @Ciudad NVARCHAR(30) = NULL,
    @CodigoPostal NVARCHAR(10) = NULL,
    @Pais NVARCHAR(30) = NULL,
    @Telefono NVARCHAR(24) = NULL,
    @Fax NVARCHAR(24) = NULL
AS
BEGIN
    INSERT INTO Proveedores (CompaniaNombre, NombreContacto, CargoContacto, Direccion, Ciudad, CodigoPostal, Pais, Telefono, Fax)
    VALUES (@CompaniaNombre, @NombreContacto, @CargoContacto, @Direccion, @Ciudad, @CodigoPostal, @Pais, @Telefono, @Fax);
    SELECT SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Actualizar
    @ProveedorID INT,
    @CompaniaNombre NVARCHAR(60),
    @NombreContacto NVARCHAR(40) = NULL,
    @CargoContacto NVARCHAR(40) = NULL,
    @Direccion NVARCHAR(80) = NULL,
    @Ciudad NVARCHAR(30) = NULL,
    @CodigoPostal NVARCHAR(10) = NULL,
    @Pais NVARCHAR(30) = NULL,
    @Telefono NVARCHAR(24) = NULL,
    @Fax NVARCHAR(24) = NULL
AS
BEGIN
    UPDATE Proveedores
    SET CompaniaNombre = @CompaniaNombre,
        NombreContacto = @NombreContacto,
        CargoContacto = @CargoContacto,
        Direccion = @Direccion,
        Ciudad = @Ciudad,
        CodigoPostal = @CodigoPostal,
        Pais = @Pais,
        Telefono = @Telefono,
        Fax = @Fax
    WHERE ProveedorID = @ProveedorID;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Eliminar
    @ProveedorID INT
AS
BEGIN
    DELETE FROM Proveedores WHERE ProveedorID = @ProveedorID;
END
GO

----------------------------------------------------
-- PRODUCTOS
----------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Producto_Listar
AS
BEGIN
    SELECT p.ProductoID, p.NombreProducto, p.ProveedorID, p.CategoriaID, 
           p.CantidadPorUnidad, p.PrecioUnidad, p.UnidadesEnExistencia, 
           p.UnidadesEnPedido, p.NivelDeReorden, p.Descontinuado,
           c.NombreCategoria, pr.CompaniaNombre AS NombreProveedor
    FROM Productos p
    LEFT JOIN Categorias c ON p.CategoriaID = c.CategoriaID
    LEFT JOIN Proveedores pr ON p.ProveedorID = pr.ProveedorID;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Producto_Crear
    @NombreProducto NVARCHAR(60),
    @ProveedorID INT = NULL,
    @CategoriaID INT = NULL,
    @CantidadPorUnidad NVARCHAR(30) = NULL,
    @PrecioUnidad DECIMAL(18,2) = 0,
    @UnidadesEnExistencia SMALLINT = 0,
    @UnidadesEnPedido SMALLINT = 0,
    @NivelDeReorden SMALLINT = 0,
    @Descontinuado BIT = 0
AS
BEGIN
    INSERT INTO Productos (NombreProducto, ProveedorID, CategoriaID, CantidadPorUnidad, PrecioUnidad, UnidadesEnExistencia, UnidadesEnPedido, NivelDeReorden, Descontinuado)
    VALUES (@NombreProducto, @ProveedorID, @CategoriaID, @CantidadPorUnidad, @PrecioUnidad, @UnidadesEnExistencia, @UnidadesEnPedido, @NivelDeReorden, @Descontinuado);
    SELECT SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Producto_Actualizar
    @ProductoID INT,
    @NombreProducto NVARCHAR(60),
    @ProveedorID INT = NULL,
    @CategoriaID INT = NULL,
    @CantidadPorUnidad NVARCHAR(30) = NULL,
    @PrecioUnidad DECIMAL(18,2) = 0,
    @UnidadesEnExistencia SMALLINT = 0,
    @UnidadesEnPedido SMALLINT = 0,
    @NivelDeReorden SMALLINT = 0,
    @Descontinuado BIT = 0
AS
BEGIN
    UPDATE Productos
    SET NombreProducto = @NombreProducto,
        ProveedorID = @ProveedorID,
        CategoriaID = @CategoriaID,
        CantidadPorUnidad = @CantidadPorUnidad,
        PrecioUnidad = @PrecioUnidad,
        UnidadesEnExistencia = @UnidadesEnExistencia,
        UnidadesEnPedido = @UnidadesEnPedido,
        NivelDeReorden = @NivelDeReorden,
        Descontinuado = @Descontinuado
    WHERE ProductoID = @ProductoID;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Producto_Eliminar
    @ProductoID INT
AS
BEGIN
    DELETE FROM Productos WHERE ProductoID = @ProductoID;
END
GO

----------------------------------------------------
-- CATALOGOS VARIOS (Dropdowns)
----------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Cliente_Listar
AS
BEGIN
    SELECT ClienteID, Empresa AS NombreCompania FROM Clientes;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Empleado_Listar
AS
BEGIN
    SELECT EmpleadoID, Apellidos + ', ' + Nombre AS NombreCompleto FROM Empleados;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Transportista_Listar
AS
BEGIN
    SELECT TransportistaID, CompaniaNombre AS NombreCompania FROM Transportistas;
END
GO

----------------------------------------------------
-- PEDIDOS Y REPORTES
----------------------------------------------------
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
    LEFT JOIN Transportistas t ON p.TransportistaID = t.TransportistaID;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Crear
    @ClienteID INT = NULL,
    @EmpleadoID INT = NULL,
    @FechaPedido DATETIME = NULL,
    @FechaRequerida DATETIME = NULL,
    @FechaEnvio DATETIME = NULL,
    @TransportistaID INT = NULL,
    @Cargo MONEY = 0,
    @Destinatario NVARCHAR(60) = NULL,
    @CiudadDestino NVARCHAR(30) = NULL,
    @PaisDestino NVARCHAR(30) = NULL
AS
BEGIN
    INSERT INTO Pedidos (ClienteID, EmpleadoID, FechaPedido, FechaRequerida, FechaEnvio, TransportistaID, Destinatario, CiudadDestino, PaisDestino)
    VALUES (@ClienteID, @EmpleadoID, @FechaPedido, @FechaRequerida, @FechaEnvio, @TransportistaID, @Destinatario, @CiudadDestino, @PaisDestino);
    SELECT SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Actualizar
    @PedidoID INT,
    @ClienteID INT = NULL,
    @EmpleadoID INT = NULL,
    @FechaPedido DATETIME = NULL,
    @FechaRequerida DATETIME = NULL,
    @FechaEnvio DATETIME = NULL,
    @TransportistaID INT = NULL,
    @Cargo MONEY = 0,
    @Destinatario NVARCHAR(60) = NULL,
    @CiudadDestino NVARCHAR(30) = NULL,
    @PaisDestino NVARCHAR(30) = NULL
AS
BEGIN
    UPDATE Pedidos
    SET ClienteID = @ClienteID,
        EmpleadoID = @EmpleadoID,
        FechaPedido = @FechaPedido,
        FechaRequerida = @FechaRequerida,
        FechaEnvio = @FechaEnvio,
        TransportistaID = @TransportistaID,
        Destinatario = @Destinatario,
        CiudadDestino = @CiudadDestino,
        PaisDestino = @PaisDestino
    WHERE PedidoID = @PedidoID;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Eliminar
    @PedidoID INT
AS
BEGIN
    DELETE FROM DetallePedidos WHERE PedidoID = @PedidoID;
    DELETE FROM Pedidos WHERE PedidoID = @PedidoID;
END
GO

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
    WHERE p.FechaPedido BETWEEN @FechaInicio AND @FechaFin;
END
GO
