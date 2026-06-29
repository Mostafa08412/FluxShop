using FastEndpoints;
using FastEndpoints.Swagger;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Shared.Middlewares;
using FluxStore.Api.Shared.Settings;
using Hangfire;
using Serilog;

namespace FluxStore.Api.Extensions
{
    public static class WebAppilicationExtensions
    {
        public static async Task UseDatebaseSetupAsync(this WebApplication app)
        {
            var databaseSetupSettings = app.Configuration.GetSection(DatabaseSetupSettings.SectionName).Get<DatabaseSetupSettings>();

            var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();

            using (var scoped = app.Services.CreateScope())

            using (var dbcontext = scoped.ServiceProvider.GetRequiredService<ApplicationDbContext>())
            {

                if (databaseSetupSettings is not null)
                {
                    if (databaseSetupSettings.ResetOnStartup)
                    {
                        Log.Information("[[[[ Resetting the database as per configuration.]]]]");

                        await dbcontext.Database.EnsureDeletedAsync();

                        Log.Information("[[[[ Database reset completed ]]]]");
                    }

                    if (databaseSetupSettings.EnsureCreated)
                    {
                        Log.Information("[[[[ Ensuring the database is created as per configuration.]]]]");

                        await dbcontext.Database.EnsureCreatedAsync();

                        Log.Information("[[[[ Database creation ensured ]]]]");
                    }

                }

            }
        }
        public static void UseCorsSettings(this WebApplication webApplication)
        {
            var corsSettings = webApplication.Configuration.GetSection(CorsSettings.SectionName).Get<CorsSettings>();
            if (corsSettings is not null)
                webApplication.UseCors(corsSettings.PolicyName);
        }
        public static void UseLocalization(this WebApplication app)
        {
            var supportedCultures = new[] { "en-US", "ar-EG" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            app.UseRequestLocalization(localizationOptions);
        }

        public static void UseHangfire(this WebApplication app)
        {
            app.UseHangfireDashboard("/hangfire");
        }
        public static void ConfigureSerilog(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, services, configuration) =>
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext());
        }

        public static async Task ConfiugreMiddlewares(this WebApplication app)
        {
            app.Lifetime.ApplicationStarted.Register(() =>
            {
                app.Logger.LogInformation("FluxStore API has fully started and is ready for requests.");
                app.Logger.LogInformation("<-------------------------------------------------------->");
            });

            app.UseGloabalExceptionHandler();

            app.UseHttpsRedirection();

            app.UseLocalization();

            app.UseCorsSettings();

            app.UseAuthErrorHandlingMiddleware();

            app.UseAuthentication();

            app.UseAuthorization();

            await app.UseDatebaseSetupAsync();

            app.UseHangfire();

            app.UseFastEndpoints(c =>
            {
                c.Endpoints.RoutePrefix = "api";
                c.Versioning.Prefix = "v";
                c.Versioning.PrependToRoute = true;
                c.Versioning.DefaultVersion = 1;
                c.Endpoints.ShortNames = false;

            }).UseSwaggerGen();
        }
    }
}
