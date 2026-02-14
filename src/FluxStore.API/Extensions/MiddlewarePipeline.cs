using FluxStore.Api.Middleware;
using FluxStore.Infrastructure.Persistence;
using Hangfire;
using Serilog;
namespace FluxStore.Api.Extensions
{
    public static class MiddlewarePipeline
    {

        public static void ConfigureMiddlewarePipeline(this WebApplication app, IConfiguration configuration)
        {
            var useSeedData = configuration.GetSection("Seeding:UseSeedData").Get<bool>();

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
            if (useSeedData)
                app.RegisterInitializer();


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


