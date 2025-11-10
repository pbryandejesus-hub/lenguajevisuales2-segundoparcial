using System.Text;
using lenguajevisuales2_segundoparcial.Data;
using lenguajevisuales2_segundoparcial.Models;

namespace lenguajevisuales2_segundoparcial.Middleware
{
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionLoggingMiddleware> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public ExceptionLoggingMiddleware(RequestDelegate next, ILogger<ExceptionLoggingMiddleware> logger, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            string requestBody = string.Empty;
            try
            {
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }
            catch { /* ignore */ }

            var originalBodyStream = context.Response.Body;
            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                // Registrar en BD
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var ip = context.Connection.RemoteIpAddress?.ToString();
                    var log = new LogApi
                    {
                        DateTime = DateTime.UtcNow,
                        TipoLog = "Error",
                        RequestBody = requestBody,
                        ResponseBody = ex.Message,
                        UrlEndpoint = $"{context.Request.Path}{context.Request.QueryString}",
                        MetodoHttp = context.Request.Method,
                        DireccionIp = ip,
                        Detalle = ex.ToString()
                    };

                    db.LogApis.Add(log);
                    db.SaveChanges();
                }
                catch (Exception logEx)
                {
                    _logger.LogError(logEx, "Error registrando el log en la base de datos");
                }

                // Return JSON error response (do not rethrow)
                try
                {
                    context.Response.Body = originalBodyStream;
                    context.Response.Clear();
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var errorObj = new
                    {
                        error = ex.Message,
                        detail = ex.ToString()
                    };

                    var json = System.Text.Json.JsonSerializer.Serialize(errorObj);
                    await context.Response.WriteAsync(json);
                }
                catch (Exception writeEx)
                {
                    _logger.LogError(writeEx, "Error escribiendo la respuesta de error");
                }
            }
        }
    }
}