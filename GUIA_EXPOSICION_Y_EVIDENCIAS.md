# Guía de exposición y evidencias del Laboratorio 04

## Explicación breve para el profesor

La aplicación usa Avalonia porque WPF no se ejecuta de forma nativa en macOS, pero mantiene C#, XAML, MVVM y ADO.NET. SQL Server 2022 se ejecuta en Docker con un volumen persistente. La aplicación no realiza SQL directo: cada repositorio abre una conexión con `Microsoft.Data.SqlClient` y llama procedimientos almacenados mediante parámetros tipados. La interfaz se divide en productos, categorías, proveedores, pedidos y reporte por fechas.

## Si preguntan cómo se conecta a la base

Puedes responder:

> Docker publica SQL Server en `localhost:1433`. La contraseña está en el archivo local `.env`, que está excluido por `.gitignore`. `DbConfig.cs` construye la cadena leyendo variables de entorno. `App.axaml.cs` inyecta esa cadena en los repositorios. Los repositorios crean `SqlConnection` y `SqlCommand`, establecen `CommandType.StoredProcedure`, agregan parámetros y usan `await using` para cerrar correctamente conexiones y lectores.

Muestra estos archivos en este orden:

1. `docker-compose.yml`: imagen, puerto, volumen y variables.
2. `.env.example`: nombres de variables sin una credencial personal.
3. `NeptunoApp/Data/DbConfig.cs`: construcción de la cadena.
4. `NeptunoApp/App.axaml.cs`: creación e inyección de repositorios.
5. `NeptunoApp/Data/ProductoRepository.cs`: conexión, comando y parámetros.

## Si preguntan dónde están los procedimientos

Están físicamente en `Database/02_Procedimientos_Almacenados.sql`. El archivo contiene:

- `usp_Producto_Listar`, `usp_Producto_ObtenerPorId`, `usp_Producto_Crear`, `usp_Producto_Actualizar`, `usp_Producto_Eliminar`.
- `usp_Categoria_Listar`, `usp_Categoria_Crear`, `usp_Categoria_Actualizar`, `usp_Categoria_Eliminar`.
- `usp_Proveedor_Listar`, `usp_Proveedor_Buscar`, `usp_Proveedor_Crear`, `usp_Proveedor_Actualizar`, `usp_Proveedor_Eliminar`.
- `usp_Cliente_Listar`, `usp_Empleado_Listar`, `usp_Transportista_Listar`.
- `usp_Pedido_Listar`, `usp_Pedido_Crear`, `usp_Pedido_Actualizar`, `usp_Pedido_Eliminar`.
- `usp_DetallePedido_ListarPorRangoFechas`.

`Scripts/setup-db.sh` ejecuta primero `01_Init_NeptunoDB.sql` y después el archivo de procedimientos dentro del contenedor. Se usa `CREATE OR ALTER PROCEDURE`, por lo que puede repetirse.

## Preparación antes de las capturas

En Terminal:

```bash
cd "/Users/antony/Documents/ANTONY 2026/6 CICLO/Desarollo web/Lab04"
docker compose up -d
./Scripts/setup-db.sh
set -a
source ./.env
set +a
/usr/local/share/dotnet/dotnet run --project NeptunoApp/NeptunoApp.csproj
```

En macOS usa `Shift + Command + 4` y luego barra espaciadora para capturar solamente la ventana. Guarda las imágenes en `Evidencias` con los nombres sugeridos.

## Capturas obligatorias

1. `01-docker-sqlserver.png`: ejecuta `docker compose ps`; debe verse `healthy` y el puerto `1433`.
2. `02-productos-listado.png`: abre Productos mostrando los cinco registros iniciales.
3. `03-productos-crear.png`: registra un producto de prueba y captura el mensaje de éxito.
4. `04-productos-editar.png`: cambia el precio o stock del producto de prueba y captura el resultado.
5. `05-productos-eliminar.png`: captura el cuadro de confirmación y luego la lista sin el producto temporal.
6. `06-categorias-crud.png`: crea una categoría temporal, edítala y captura la tabla; después elimínala.
7. `07-proveedores-busqueda.png`: filtra contacto `Ana` y ciudad `Lima`; captura filtros y resultado.
8. `08-proveedores-crud.png`: crea y edita un proveedor temporal; captura la tabla y luego elimínalo.
9. `09-pedidos-listado.png`: abre Pedidos y muestra clientes, empleados, transportistas y totales.
10. `10-pedidos-crud.png`: crea un pedido temporal, edítalo, captura el resultado y luego elimínalo.
11. `11-reporte-con-datos.png`: consulta del `01/08/2026` al `31/08/2026`; deben aparecer detalles y total.
12. `12-reporte-sin-datos.png`: consulta un mes futuro; debe mostrarse el mensaje sin resultados.
13. `13-reporte-fechas-invalidas.png`: coloca la fecha inicial después de la final; captura la validación visible.
14. `14-compilacion.png`: ejecuta `dotnet build`; captura 0 errores y 0 advertencias.

## Evidencia de procedimientos desde Terminal

```bash
set -a; source ./.env; set +a
docker exec neptuno-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -d NeptunoDB \
  -Q "SELECT name FROM sys.procedures WHERE name LIKE 'usp_%' ORDER BY name"
```

Toma la captura `15-procedimientos-almacenados.png`. La contraseña no se imprime porque solo se expande como argumento.

## Texto para las secciones del documento

**Repositorio:** pegar aquí la URL del repositorio del equipo después de crearlo y publicarlo.

**Explicación:** La solución implementa los mantenimientos solicitados mediante Avalonia y MVVM. Los repositorios usan ADO.NET puro y procedimientos almacenados parametrizados contra SQL Server en Docker. La configuración se obtiene de variables de entorno y las conexiones se liberan de forma asincrónica.

**Observaciones:** En Apple Silicon SQL Server requiere `platform: linux/amd64`. El script original tenía caracteres mal codificados y no era reejecutable; por ello se conservó intacto y se agregó una versión segura con acentos corregidos. Las eliminaciones respetan las claves foráneas y la eliminación de pedidos utiliza una transacción.

**Conclusiones:**

1. Aprendí a separar la interfaz, la lógica y el acceso a datos usando MVVM.
2. Comprobé que los procedimientos almacenados permiten centralizar las operaciones de la base.
3. Utilicé parámetros para evitar concatenar valores dentro del SQL.
4. Logré ejecutar SQL Server en mi Mac mediante Docker y conservar los datos en un volumen.
5. Verifiqué los mantenimientos, la búsqueda y el reporte con datos reales de NeptunoDB.
