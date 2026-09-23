USE NeptunoDB;
GO
CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Listar AS
BEGIN SET NOCOUNT ON; SELECT CategoriaID, NombreCategoria FROM dbo.Categorias ORDER BY NombreCategoria; END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Listar AS
BEGIN SET NOCOUNT ON; SELECT ProveedorID, CompaniaNombre FROM dbo.Proveedores ORDER BY CompaniaNombre; END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Producto_Listar AS
BEGIN
 SET NOCOUNT ON;
 SELECT p.ProductoID,p.NombreProducto,p.ProveedorID,p.CategoriaID,p.CantidadPorUnidad,
 p.PrecioUnidad,p.UnidadesEnExistencia,p.UnidadesEnPedido,p.NivelDeReorden,p.Descontinuado,
 c.NombreCategoria,pr.CompaniaNombre AS NombreProveedor
 FROM dbo.Productos p
 LEFT JOIN dbo.Categorias c ON c.CategoriaID=p.CategoriaID
 LEFT JOIN dbo.Proveedores pr ON pr.ProveedorID=p.ProveedorID
 ORDER BY p.NombreProducto;
END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Producto_ObtenerPorId @ProductoID INT AS
BEGIN SET NOCOUNT ON; SELECT * FROM dbo.Productos WHERE ProductoID=@ProductoID; END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Producto_Crear
 @NombreProducto NVARCHAR(60), @ProveedorID INT=NULL, @CategoriaID INT=NULL,
 @CantidadPorUnidad NVARCHAR(30)=NULL, @PrecioUnidad DECIMAL(10,2)=0,
 @UnidadesEnExistencia SMALLINT=0, @UnidadesEnPedido SMALLINT=0,
 @NivelDeReorden SMALLINT=0, @Descontinuado BIT=0
AS
BEGIN
 SET NOCOUNT ON;
 IF NULLIF(LTRIM(RTRIM(@NombreProducto)),N'') IS NULL THROW 51001,N'El nombre del producto es obligatorio.',1;
 IF @PrecioUnidad<0 OR @UnidadesEnExistencia<0 OR @UnidadesEnPedido<0 OR @NivelDeReorden<0 THROW 51002,N'Los valores numéricos no pueden ser negativos.',1;
 INSERT dbo.Productos (NombreProducto,ProveedorID,CategoriaID,CantidadPorUnidad,PrecioUnidad,UnidadesEnExistencia,UnidadesEnPedido,NivelDeReorden,Descontinuado)
 VALUES (LTRIM(RTRIM(@NombreProducto)),@ProveedorID,@CategoriaID,NULLIF(LTRIM(RTRIM(@CantidadPorUnidad)),N''),@PrecioUnidad,@UnidadesEnExistencia,@UnidadesEnPedido,@NivelDeReorden,@Descontinuado);
 SELECT CONVERT(INT,SCOPE_IDENTITY()) AS ProductoID;
END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Producto_Actualizar
 @ProductoID INT, @NombreProducto NVARCHAR(60), @ProveedorID INT=NULL, @CategoriaID INT=NULL,
 @CantidadPorUnidad NVARCHAR(30)=NULL, @PrecioUnidad DECIMAL(10,2)=0,
 @UnidadesEnExistencia SMALLINT=0, @UnidadesEnPedido SMALLINT=0,
 @NivelDeReorden SMALLINT=0, @Descontinuado BIT=0
AS
BEGIN
 SET NOCOUNT ON;
 IF NULLIF(LTRIM(RTRIM(@NombreProducto)),N'') IS NULL THROW 51003,N'El nombre del producto es obligatorio.',1;
 IF @PrecioUnidad<0 OR @UnidadesEnExistencia<0 OR @UnidadesEnPedido<0 OR @NivelDeReorden<0 THROW 51004,N'Los valores numéricos no pueden ser negativos.',1;
 UPDATE dbo.Productos SET NombreProducto=LTRIM(RTRIM(@NombreProducto)),ProveedorID=@ProveedorID,CategoriaID=@CategoriaID,
 CantidadPorUnidad=NULLIF(LTRIM(RTRIM(@CantidadPorUnidad)),N''),PrecioUnidad=@PrecioUnidad,
 UnidadesEnExistencia=@UnidadesEnExistencia,UnidadesEnPedido=@UnidadesEnPedido,
 NivelDeReorden=@NivelDeReorden,Descontinuado=@Descontinuado WHERE ProductoID=@ProductoID;
 IF @@ROWCOUNT=0 THROW 51005,N'El producto no existe.',1;
END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Producto_Eliminar @ProductoID INT AS
BEGIN
 SET NOCOUNT ON;
 IF EXISTS(SELECT 1 FROM dbo.DetallePedidos WHERE ProductoID=@ProductoID) THROW 51006,N'No se puede eliminar porque el producto pertenece a un pedido.',1;
 DELETE dbo.Productos WHERE ProductoID=@ProductoID;
 IF @@ROWCOUNT=0 THROW 51007,N'El producto no existe.',1;
END
GO

/* CRUD DE CATEGORIAS */
CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Listar AS
BEGIN SET NOCOUNT ON; SELECT CategoriaID,NombreCategoria,Descripcion FROM dbo.Categorias ORDER BY NombreCategoria; END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Crear @NombreCategoria NVARCHAR(30),@Descripcion NVARCHAR(200)=NULL AS
BEGIN SET NOCOUNT ON; IF NULLIF(LTRIM(RTRIM(@NombreCategoria)),N'') IS NULL THROW 51101,N'El nombre es obligatorio.',1;
 INSERT dbo.Categorias(NombreCategoria,Descripcion) VALUES(LTRIM(RTRIM(@NombreCategoria)),NULLIF(LTRIM(RTRIM(@Descripcion)),N'')); SELECT CONVERT(INT,SCOPE_IDENTITY()); END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Actualizar @CategoriaID INT,@NombreCategoria NVARCHAR(30),@Descripcion NVARCHAR(200)=NULL AS
BEGIN SET NOCOUNT ON; UPDATE dbo.Categorias SET NombreCategoria=LTRIM(RTRIM(@NombreCategoria)),Descripcion=NULLIF(LTRIM(RTRIM(@Descripcion)),N'') WHERE CategoriaID=@CategoriaID;
 IF @@ROWCOUNT=0 THROW 51102,N'La categoría no existe.',1; END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Eliminar @CategoriaID INT AS
BEGIN SET NOCOUNT ON; IF EXISTS(SELECT 1 FROM dbo.Productos WHERE CategoriaID=@CategoriaID) THROW 51103,N'No se puede eliminar porque tiene productos asociados.',1;
 DELETE dbo.Categorias WHERE CategoriaID=@CategoriaID; IF @@ROWCOUNT=0 THROW 51104,N'La categoría no existe.',1; END
GO

/* CRUD Y BUSQUEDA DE PROVEEDORES */
CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Listar AS
BEGIN SET NOCOUNT ON; SELECT ProveedorID,CompaniaNombre,NombreContacto,CargoContacto,Direccion,Ciudad,CodigoPostal,Pais,Telefono,Fax FROM dbo.Proveedores ORDER BY CompaniaNombre; END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Buscar @NombreContacto NVARCHAR(40)=NULL,@Ciudad NVARCHAR(30)=NULL AS
BEGIN SET NOCOUNT ON; SET @NombreContacto=NULLIF(LTRIM(RTRIM(@NombreContacto)),N''); SET @Ciudad=NULLIF(LTRIM(RTRIM(@Ciudad)),N'');
 SELECT ProveedorID,CompaniaNombre,NombreContacto,CargoContacto,Direccion,Ciudad,CodigoPostal,Pais,Telefono,Fax FROM dbo.Proveedores
 WHERE (@NombreContacto IS NULL OR NombreContacto LIKE N'%'+@NombreContacto+N'%') AND (@Ciudad IS NULL OR Ciudad LIKE N'%'+@Ciudad+N'%') ORDER BY CompaniaNombre; END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Crear
 @CompaniaNombre NVARCHAR(60),@NombreContacto NVARCHAR(40)=NULL,@CargoContacto NVARCHAR(40)=NULL,@Direccion NVARCHAR(80)=NULL,
 @Ciudad NVARCHAR(30)=NULL,@CodigoPostal NVARCHAR(10)=NULL,@Pais NVARCHAR(30)=NULL,@Telefono NVARCHAR(24)=NULL,@Fax NVARCHAR(24)=NULL AS
BEGIN SET NOCOUNT ON; IF NULLIF(LTRIM(RTRIM(@CompaniaNombre)),N'') IS NULL THROW 51201,N'La compañía es obligatoria.',1;
 INSERT dbo.Proveedores VALUES(LTRIM(RTRIM(@CompaniaNombre)),NULLIF(@NombreContacto,N''),NULLIF(@CargoContacto,N''),NULLIF(@Direccion,N''),NULLIF(@Ciudad,N''),NULLIF(@CodigoPostal,N''),NULLIF(@Pais,N''),NULLIF(@Telefono,N''),NULLIF(@Fax,N'')); SELECT CONVERT(INT,SCOPE_IDENTITY()); END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Actualizar
 @ProveedorID INT,@CompaniaNombre NVARCHAR(60),@NombreContacto NVARCHAR(40)=NULL,@CargoContacto NVARCHAR(40)=NULL,@Direccion NVARCHAR(80)=NULL,
 @Ciudad NVARCHAR(30)=NULL,@CodigoPostal NVARCHAR(10)=NULL,@Pais NVARCHAR(30)=NULL,@Telefono NVARCHAR(24)=NULL,@Fax NVARCHAR(24)=NULL AS
BEGIN SET NOCOUNT ON; UPDATE dbo.Proveedores SET CompaniaNombre=LTRIM(RTRIM(@CompaniaNombre)),NombreContacto=NULLIF(@NombreContacto,N''),CargoContacto=NULLIF(@CargoContacto,N''),Direccion=NULLIF(@Direccion,N''),Ciudad=NULLIF(@Ciudad,N''),CodigoPostal=NULLIF(@CodigoPostal,N''),Pais=NULLIF(@Pais,N''),Telefono=NULLIF(@Telefono,N''),Fax=NULLIF(@Fax,N'') WHERE ProveedorID=@ProveedorID;
 IF @@ROWCOUNT=0 THROW 51202,N'El proveedor no existe.',1; END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Eliminar @ProveedorID INT AS
BEGIN SET NOCOUNT ON; IF EXISTS(SELECT 1 FROM dbo.Productos WHERE ProveedorID=@ProveedorID) THROW 51203,N'No se puede eliminar porque tiene productos asociados.',1;
 DELETE dbo.Proveedores WHERE ProveedorID=@ProveedorID; IF @@ROWCOUNT=0 THROW 51204,N'El proveedor no existe.',1; END
GO

/* CATALOGOS Y CRUD DE PEDIDOS */
CREATE OR ALTER PROCEDURE dbo.usp_Cliente_Listar AS BEGIN SET NOCOUNT ON; SELECT ClienteID,Empresa FROM dbo.Clientes ORDER BY Empresa; END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Empleado_Listar AS BEGIN SET NOCOUNT ON; SELECT EmpleadoID,Nombre+N' '+Apellidos NombreCompleto FROM dbo.Empleados ORDER BY Apellidos,Nombre; END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Transportista_Listar AS BEGIN SET NOCOUNT ON; SELECT TransportistaID,CompaniaNombre FROM dbo.Transportistas ORDER BY CompaniaNombre; END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Listar AS
BEGIN SET NOCOUNT ON; SELECT p.PedidoID,p.ClienteID,p.EmpleadoID,p.FechaPedido,p.FechaRequerida,p.FechaEnvio,p.TransportistaID,p.Destinatario,p.CiudadDestino,p.PaisDestino,
 c.Empresa NombreCliente,e.Nombre+N' '+e.Apellidos NombreEmpleado,t.CompaniaNombre NombreTransportista,
 ISNULL((SELECT SUM(d.PrecioUnidad*d.Cantidad*(1-d.Descuento)) FROM dbo.DetallePedidos d WHERE d.PedidoID=p.PedidoID),0) Total
 FROM dbo.Pedidos p LEFT JOIN dbo.Clientes c ON c.ClienteID=p.ClienteID LEFT JOIN dbo.Empleados e ON e.EmpleadoID=p.EmpleadoID LEFT JOIN dbo.Transportistas t ON t.TransportistaID=p.TransportistaID ORDER BY p.FechaPedido DESC,p.PedidoID DESC; END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Crear @ClienteID INT=NULL,@EmpleadoID INT=NULL,@FechaPedido DATE,@FechaRequerida DATE=NULL,@FechaEnvio DATE=NULL,@TransportistaID INT=NULL,@Destinatario NVARCHAR(60)=NULL,@CiudadDestino NVARCHAR(30)=NULL,@PaisDestino NVARCHAR(30)=NULL AS
BEGIN SET NOCOUNT ON; IF @FechaRequerida<@FechaPedido THROW 51301,N'La fecha requerida no puede ser anterior.',1;
 INSERT dbo.Pedidos VALUES(@ClienteID,@EmpleadoID,@FechaPedido,@FechaRequerida,@FechaEnvio,@TransportistaID,NULLIF(@Destinatario,N''),NULLIF(@CiudadDestino,N''),NULLIF(@PaisDestino,N'')); SELECT CONVERT(INT,SCOPE_IDENTITY()); END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Actualizar @PedidoID INT,@ClienteID INT=NULL,@EmpleadoID INT=NULL,@FechaPedido DATE,@FechaRequerida DATE=NULL,@FechaEnvio DATE=NULL,@TransportistaID INT=NULL,@Destinatario NVARCHAR(60)=NULL,@CiudadDestino NVARCHAR(30)=NULL,@PaisDestino NVARCHAR(30)=NULL AS
BEGIN SET NOCOUNT ON; IF @FechaRequerida<@FechaPedido THROW 51302,N'La fecha requerida no puede ser anterior.',1;
 UPDATE dbo.Pedidos SET ClienteID=@ClienteID,EmpleadoID=@EmpleadoID,FechaPedido=@FechaPedido,FechaRequerida=@FechaRequerida,FechaEnvio=@FechaEnvio,TransportistaID=@TransportistaID,Destinatario=NULLIF(@Destinatario,N''),CiudadDestino=NULLIF(@CiudadDestino,N''),PaisDestino=NULLIF(@PaisDestino,N'') WHERE PedidoID=@PedidoID;
 IF @@ROWCOUNT=0 THROW 51303,N'El pedido no existe.',1; END
GO
CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Eliminar @PedidoID INT AS
BEGIN SET NOCOUNT ON; BEGIN TRY BEGIN TRANSACTION; DELETE dbo.DetallePedidos WHERE PedidoID=@PedidoID; DELETE dbo.Pedidos WHERE PedidoID=@PedidoID; IF @@ROWCOUNT=0 THROW 51304,N'El pedido no existe.',1; COMMIT; END TRY BEGIN CATCH IF XACT_STATE()<>0 ROLLBACK; THROW; END CATCH END
GO

/* REPORTE OBLIGATORIO: DETALLES INNER JOIN PEDIDOS Y FILTRO DE FECHAS */
CREATE OR ALTER PROCEDURE dbo.usp_DetallePedido_ListarPorRangoFechas @FechaInicio DATE,@FechaFin DATE AS
BEGIN SET NOCOUNT ON; IF @FechaInicio>@FechaFin THROW 51401,N'La fecha inicial no puede ser posterior a la final.',1;
 SELECT p.PedidoID,p.FechaPedido,c.Empresa NombreCliente,pr.NombreProducto,d.PrecioUnidad,d.Cantidad,d.Descuento,
 d.PrecioUnidad*d.Cantidad*(1-d.Descuento) Subtotal
 FROM dbo.DetallePedidos d INNER JOIN dbo.Pedidos p ON p.PedidoID=d.PedidoID
 INNER JOIN dbo.Productos pr ON pr.ProductoID=d.ProductoID LEFT JOIN dbo.Clientes c ON c.ClienteID=p.ClienteID
 WHERE p.FechaPedido BETWEEN @FechaInicio AND @FechaFin ORDER BY p.FechaPedido,p.PedidoID,pr.NombreProducto; END
GO
