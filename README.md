# Laboratorio 04 NeptunoDB con ADO.NET

Aplicación de escritorio multiplataforma para administrar productos, categorías, proveedores y pedidos de `NeptunoDB`, además de buscar proveedores y reportar detalles por fechas. Conserva el enfoque del ejemplo del profesor —modelos, repositorios ADO.NET y ViewModels— y reemplaza WPF por Avalonia para funcionar en macOS y Windows.

## Requisitos

- macOS Apple Silicon o Windows
- Docker Desktop
- .NET SDK 10.0.401 o compatible
- Rider 2026 o compatible

## 1. Configurar SQL Server

Comprueba primero si el puerto está libre:

```bash
lsof -nP -iTCP:1433 -sTCP:LISTEN
```

Crea el archivo local de entorno y cambia la contraseña de ejemplo:

```bash
cp .env.example .env
```

Si `1433` está ocupado, configura por ejemplo `MSSQL_PORT=1434` en `.env`. El contenedor usa `platform: linux/amd64`, porque SQL Server se ejecuta mediante emulación en Apple Silicon. Los datos se conservan en el volumen `neptuno_sql_data`.

Inicia el servidor y prepara la base:

```bash
docker compose up -d
./Scripts/setup-db.sh
```

`Database/NeptunoDB.original.sql` es la copia del archivo entregado. `Database/01_Init_NeptunoDB.sql` conserva su esquema real, corrige los acentos y permite volver a ejecutarlo. `Database/02_Procedimientos_Almacenados.sql` contiene todos los procedimientos parametrizados solicitados.

## 2. Configurar la aplicación

La aplicación no guarda contraseñas. Exporta las variables antes de abrirla:

```bash
set -a
source .env
set +a
export MSSQL_HOST=localhost
```

También puedes definir directamente `NEPTUNO_CONNECTION_STRING`. Si cambiaste el puerto en `.env`, la aplicación lo leerá mediante `MSSQL_PORT`.

## 3. Restaurar y ejecutar

```bash
/usr/local/share/dotnet/dotnet restore NeptunoLab04.slnx
/usr/local/share/dotnet/dotnet build NeptunoLab04.slnx
/usr/local/share/dotnet/dotnet run --project NeptunoApp/NeptunoApp.csproj
```

En Rider, abre `NeptunoLab04.slnx`, selecciona `NeptunoApp` y ejecuta. Inicia Rider desde una terminal con las variables exportadas o añádelas a la configuración de ejecución.

## Arquitectura

- `Models`: representa productos, categorías, proveedores, pedidos, catálogos y líneas de reporte con los nombres reales del script.
- `Data`: crea conexiones cortas, ejecuta procedimientos almacenados parametrizados y mapea resultados.
- `ViewModels`: contiene estado, validación y comandos asincrónicos de cada módulo.
- `Views`: presenta navegación lateral, formularios, tablas, carga, mensajes y confirmaciones.
- `Database`: conserva el script original y los scripts reejecutables.
- `Evidencias`: lugar reservado para capturas de la ejecución.

## Relación con los requisitos

| Requisito | Implementación |
|---|---|
| NeptunoDB y esquema real | `Database/01_Init_NeptunoDB.sql` |
| CRUD de productos | `Database/02_Procedimientos_Almacenados.sql`, `ProductoRepository.cs` |
| CRUD de categorías | `CategoriaRepository.cs`, `CategoriasViewModel.cs`, `CategoriasView.axaml` |
| CRUD de proveedores | `ProveedorRepository.cs`, `ProveedoresViewModel.cs`, `ProveedoresView.axaml` |
| Buscar por contacto y ciudad | `usp_Proveedor_Buscar` y la franja de filtros de `ProveedoresView.axaml` |
| CRUD de pedidos | `PedidoRepository.cs`, `PedidosViewModel.cs`, `PedidosView.axaml` |
| Eliminación transaccional de pedido y detalle | `usp_Pedido_Eliminar` |
| Reporte por intervalo con INNER JOIN | `usp_DetallePedido_ListarPorRangoFechas`, `ReportesView.axaml` |
| ADO.NET puro y parámetros | `NeptunoApp/Data/ProductoRepository.cs` |
| Categoría y proveedor en listas | `MainWindow.axaml`, procedimientos de catálogo |
| Validación de obligatorios y números | `ProductosViewModel.cs` |
| Listar, registrar, editar y eliminar | comandos de `ProductosViewModel.cs` y tabla de `MainWindow.axaml` |
| Confirmación antes de eliminar | `MainWindow.axaml.cs` |
| Carga, errores y estado vacío | `MainWindow.axaml` y `ProductosViewModel.cs` |
| Docker Apple Silicon y persistencia | `docker-compose.yml` |
| Credenciales externas | `.env.example`, `.gitignore`, `DbConfig.cs` |

## Cómo funciona la conexión

Docker publica el puerto configurado en `.env` hacia el puerto `1433` interno de SQL Server. `DbConfig.GetConnectionString()` lee `MSSQL_HOST`, `MSSQL_PORT`, `MSSQL_USER` y `MSSQL_SA_PASSWORD`; no existe una contraseña escrita en C#. `App.axaml.cs` entrega esa cadena a cada repositorio. Cada método crea `SqlConnection`, configura un `SqlCommand` con `CommandType.StoredProcedure`, agrega parámetros tipados, abre la conexión de manera asincrónica y la libera con `await using`.

## Observaciones

El script original solo puede ejecutarse una vez porque crea las tablas directamente. Por eso conservo ese archivo sin modificar y uso una versión segura para el entorno Docker. La eliminación de un producto se rechaza cuando ya pertenece a `DetallePedidos`, respetando la relación del esquema.

## Conclusiones

1. Separé la interfaz del acceso a datos para mantener cada parte con claridad.
2. Usé procedimientos almacenados y parámetros en todas las operaciones del producto.
3. Evité guardar la contraseña en el repositorio o en el código fuente.
4. Adapté el patrón del profesor a Avalonia sin cambiar la lógica principal de ADO.NET.
5. Dejé Docker y los scripts preparados para repetir la instalación sin perder el volumen.

## División sugerida para dos integrantes

- Integrante A: `Database`, `Data`, configuración Docker, categorías y proveedores.
- Integrante B: `Models`, productos, pedidos, reportes y evidencias visuales.

Conviene trabajar en ramas `feature/datos-productos` y `feature/ui-productos`. Eviten editar simultáneamente `README.md` y `NeptunoApp.csproj`; intégrenlos al final en una rama común.
