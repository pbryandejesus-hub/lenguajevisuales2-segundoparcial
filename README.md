# lenguajesvisuales2-segundoparcial

API REST en ASP.NET Core (.NET 8) para registro de clientes con fotos, subida y extracción de ZIPs asociados a clientes, y registro de logs de la API.

Requerimientos implementados:
- Registro de clientes (POST `/api/clientes`) con tres fotos almacenadas en BD (`varbinary(max)`).
- Carga de múltiples archivos vía `.zip` (POST `/api/archivos/upload-zip?ci={ci}`), extracción a `wwwroot/uploads/{ci}` y registro en tabla `ArchivoClientes`.
- Registro de errores y eventos en tabla `LogApis` mediante middleware global.
- Endpoints para consultar clientes, archivos y logs.
- Project configured for EF Core (Code First) and Swagger.

Instalación y ejecución:
1. Actualiza `appsettings.json` con tu cadena de conexión bajo `ConnectionStrings:DefaultConnection`.
2. Instala la herramienta EF (si no la tienes):
   - `dotnet tool install --global dotnet-ef`
3. Restaurar paquetes y aplicar migraciones:
   - `dotnet restore`
   - `dotnet ef database update` (la migración `InitialCreate` ya está incluida en `Migrations/`)
4. Ejecutar:
   - `dotnet run`
5. Abrir Swagger: `https://localhost:5001/swagger` (o la URL que indique la consola).

Estructura relevante:
- `Controllers/ClientesController.cs` — registro de clientes
- `Controllers/ArchivosController.cs` — subida y extracción de ZIP
- `Controllers/LogsController.cs` — consulta de logs
- `Data/ApplicationDbContext.cs` — DbContext y entidades
- `Models/` — `Cliente`, `ArchivoCliente`, `LogApi`
- `Middleware/ExceptionLoggingMiddleware.cs` — captura de excepciones y almacenamiento de logs
- `Services/FileService.cs` — manejo de archivos
- `Migrations/20251110_InitialCreate.cs` — migración inicial (incluida)

Entrega:
- Sube el repositorio a GitHub con nombre `lenguajesvisuales2-segundoparcial` y adjunta evidencias de pruebas en `PRUEBAS.md`.
