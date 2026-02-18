using FluxStore.Api.Middleware;
using FluxStore.Infrastructure.Persistence;
using Hangfire;
using Serilog;
namespace FluxStore.Api.Extensions
{
    public static class MiddlewarePipeline
    {
        public static void UseLocalization(this WebApplication app)
        {
            var supportedCultures = new[] { "en-US", "ar-EG" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            app.UseRequestLocalization(localizationOptions);
        }


        public static void ConfigureMiddlewarePipeline(this WebApplication app, IConfiguration configuration)
        {
            var resetDatabase = configuration.GetSection("InitializeDatabase:ResetDatabase").Get<bool>();
            var InitialDatabase = configuration.GetSection("InitializeDatabase:InitializeDatabase").Get<bool>();
            var seedData = configuration.GetSection("InitializeDatabase:SeedData").Get<bool>();

            app.UseGloabalExceptionHandler();

            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {

                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Version 1");
                    options.DisplayRequestDuration();
                });
            }

            if (resetDatabase)
                app.ResetDatabaseIfExists().Wait();

            if (InitialDatabase)
                app.InitializeDatabase().Wait();

            if (seedData)
                app.SeedData().Wait();

            app.UseLocalization();
            app.UseHangfireDashboard();
            app.UseCors(configuration.GetSection("CorsSettings:PolicyName").Get<string>());
            app.UseRouting();
            app.UseMiddleware<HandleAuthenticationErrorMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

        }


    }

}


