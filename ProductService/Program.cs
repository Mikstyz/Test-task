using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using Service;
using gRCP;

public class ProgProduct
{
    public static async Task Main(string[] args)
    {
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", false);

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

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenLocalhost(5004, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1; // Только HTTP/1.1
                });
            });


            builder.Services.AddControllers();
            builder.Host.UseSerilog();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthorization();

            // Добавляем поддержку gRPC
            builder.Services.AddScoped<ProductManager>();
            builder.Services.AddScoped<grspManager>();
            builder.Services.AddGrpc();

            // Настройка Kestrel для работы только с HTTP/1.1
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ConfigureHttpsDefaults(config =>
                {
                    config.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                });

                // Отключаем HTTP/2, используем только HTTP/1.1
                options.ListenAnyIP(5005, listenOptions =>
                {
                    listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
                });
            });

            var app = builder.Build();

            app.Urls.Add("http://localhost:5005");

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                c.RoutePrefix = string.Empty;
            });

            app.MapControllers();

            // Регистрируем gRPC-сервис
            app.MapGrpcService<ProductServiceImpl>();

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
