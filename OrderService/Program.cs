using Serilog;
using Service;

public class ProgOrder
{
    public static async Task Main(string[] args)
    {
        await Hosting(args);
    }

    public static async Task Hosting(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            Log.Information("app start");

            var builder = WebApplication.CreateBuilder(args);

            // Используем Serilog для логирования
            builder.Host.UseSerilog();

            // Добавляем сервисы
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpClient<OrderService>();

            builder.Services.AddAuthorization();
            builder.Services.AddControllers();

            // Настройка Kestrel для работы только с HTTP/1.1 и портом 5006
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ConfigureHttpsDefaults(config =>
                {
                    config.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                });

                // Отключаем HTTP/2
                options.ListenAnyIP(5006, listenOptions =>
                {
                    listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
                });
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // Используем Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                c.RoutePrefix = string.Empty;
            });

            // Основной роутинг и обработка контроллеров
            app.UseRouting();
            app.MapControllers();

            // Запуск приложения
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "App down");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
