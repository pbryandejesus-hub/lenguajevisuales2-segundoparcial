using lenguajevisuales2_segundoparcial.Data;
using lenguajevisuales2_segundoparcial.Middleware;
using lenguajevisuales2_segundoparcial.Models;
using lenguajevisuales2_segundoparcial.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Servicios de archivos
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddDirectoryBrowser();

var app = builder.Build();

// Middleware de logging
app.UseMiddleware<ExceptionLoggingMiddleware>();

// Habilitar Swagger en producción y MonsterASP
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API v1");
    c.RoutePrefix = string.Empty; // / abre Swagger
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Seed datos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var fileService = services.GetRequiredService<IFileService>();

        db.Database.EnsureCreated();

        if (!db.Clientes.Any())
        {
            var ci = "12345678";
            var cliente = new Cliente
            {
                CI = ci,
                Nombres = "Juan Perez",
                Direccion = "Calle Falsa 123",
                Telefono = "1234567",
                FotoCasa1 = Encoding.UTF8.GetBytes("FotoCasa1 de prueba"),
                FotoCasa2 = Encoding.UTF8.GetBytes("FotoCasa2 de prueba"),
                FotoCasa3 = Encoding.UTF8.GetBytes("FotoCasa3 de prueba")
            };

            db.Clientes.Add(cliente);
            db.SaveChanges();

            var uploadsRoot = fileService.EnsureUploadsFolder();
            var clienteFolder = Path.Combine(uploadsRoot, ci);
            if (!Directory.Exists(clienteFolder)) Directory.CreateDirectory(clienteFolder);

            var file1 = Path.Combine(clienteFolder, "documento_prueba.txt");
            File.WriteAllText(file1, "Contenido de prueba del documento 1");

            var file2 = Path.Combine(clienteFolder, "imagen_prueba.jpg");
            File.WriteAllText(file2, "Contenido ficticio imagen");

            db.ArchivoClientes.Add(new ArchivoCliente { CICliente = ci, NombreArchivo = "documento_prueba.txt", UrlArchivo = $"/uploads/{ci}/documento_prueba.txt" });
            db.ArchivoClientes.Add(new ArchivoCliente { CICliente = ci, NombreArchivo = "imagen_prueba.jpg", UrlArchivo = $"/uploads/{ci}/imagen_prueba.jpg" });

            db.LogApis.Add(new LogApi
            {
                DateTime = DateTime.UtcNow,
                TipoLog = "Info",
                RequestBody = "Seed data",
                ResponseBody = "Cliente y archivos creados",
                UrlEndpoint = "/seed",
                MetodoHttp = "SEED",
                DireccionIp = "127.0.0.1",
                Detalle = "Seed inicial de datos de prueba"
            });

            db.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error seeding database");
    }
}

app.Run();
