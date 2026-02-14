using FluxStore.Api.Extensions;
using Serilog;

namespace FluxStore.Api.FluxStore.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LogBootstrapper.CreateBootstrapLogger();

            try
            {
                Log.Information("Starting FluxStore API...");

                var builder = WebApplication.CreateBuilder(args);

                builder.ConfigureSerilog();

                builder.Services.RegisterAllServices(builder.Configuration);

                var app = builder.Build();

                app.ConfigureMiddlewarePipeline(builder.Configuration);

                Log.Information("FluxStore API started successfully");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
