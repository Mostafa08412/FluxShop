using FluxStore.Api.Extensions;
using Serilog;
namespace FluxStore.Api
{
    public class Program
    {

        public static async Task Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

            var builder = WebApplication.CreateBuilder(args);

            builder.ConfigureSerilog();

            try
            {
                Log.Logger.Information("Starting the FluxStore API application...");
                Log.Logger.Information("<-------------------------------------------------------->");

                builder.Services.RegisterServices(builder.Configuration);

                var app = builder.Build();

                await app.ConfiugreMiddlewares();

                app.Run();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();

            }
        }
    }
}
