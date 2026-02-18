using FluxStore.Application.Common.Errors;
using FluxStore.Application.Common.Interfaces;
using FluxStore.Domain.Abstractions;
using FluxStore.Domain.Users;
using FluxStore.Infrastructure.Authentication;
using FluxStore.Infrastructure.Common;
using FluxStore.Infrastructure.Common.Exceptions;
using FluxStore.Infrastructure.EmailServices;
using FluxStore.Infrastructure.EmailServices.Options;
using FluxStore.Infrastructure.FileManager;
using FluxStore.Infrastructure.Persistence;
using FluxStore.Infrastructure.Persistence.BackgroundJobs;
using FluxStore.Infrastructure.Persistence.Identity;
using FluxStore.Infrastructure.Persistence.Repositories;
using FluxStore.Infrastructure.Tokens;
using FluxStore.Infrastructure.Tokens.Options;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
namespace FluxStore.Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddHangFireBackgroundJobWorker(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new MissingConfigurationSettingsException("DefaultConnection");

            services.AddHangfire(X => X.UseSqlServerStorage(connectionString, new SqlServerStorageOptions
            {
                PrepareSchemaIfNecessary = true

            }));


            services.AddHangfireServer();

            services.AddScoped<BackgroundJobBridge>();

            services.AddScoped<IBackgroundJobWorker, BackgroundJobWorker>();

            return services;
        }
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAuthorizationBuilder();

            services.AddOptions<TokenSettings>().Bind(configuration.GetSection(TokenSettings.SectionName));



            var section = configuration.GetSection(TokenSettings.SectionName);

            var tokenSettings = section.Get<TokenSettings>();


            if (tokenSettings is null)
                throw new MissingConfigurationSettingsException(TokenSettings.SectionName);

            if (string.IsNullOrWhiteSpace(tokenSettings.SecretKey))
                throw new MissingConfigurationSettingsException(nameof(tokenSettings.SecretKey));

            if (string.IsNullOrWhiteSpace(tokenSettings.Issuer))
                throw new MissingConfigurationSettingsException(nameof(tokenSettings.Issuer));

            if (string.IsNullOrWhiteSpace(tokenSettings.Audience))
                throw new MissingConfigurationSettingsException(nameof(tokenSettings.Audience));


            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.SecretKey));





            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = tokenSettings.Issuer,
                        ValidAudience = tokenSettings.Audience,
                        IssuerSigningKey = symmetricKey,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    options.IncludeErrorDetails = true; // Enable detailed errors


                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.HttpContext.Request.Path.Value?.Contains("hubs") == true)
                            {
                                var accessToken = context.HttpContext.Request.Query["access_Token"];
                                if (!string.IsNullOrWhiteSpace(accessToken))
                                {
                                    context.HttpContext.Request.Headers.TryAdd("Authorization", $"Bearer {accessToken}");

                                }
                            }

                            return Task.CompletedTask;
                        }
                        ,

                        OnChallenge = context =>
                        {
                            if (context.Response.HasStarted)
                                return Task.CompletedTask;

                            else if (context.AuthenticateFailure is SecurityTokenInvalidSignatureException)
                            {
                                context.Response.Headers.TryAdd("Auth-Fail-Type", ApplicationErrors.IdentityErrors.InvalidToken.Code);
                                context.HandleResponse();
                                return Task.CompletedTask;
                            }


                            else if (context.AuthenticateFailure is SecurityTokenExpiredException)
                            {
                                context.Response.Headers.TryAdd("Auth-Fail-Type", ApplicationErrors.IdentityErrors.ExpiredToken.Code);
                                context.HandleResponse();
                                return Task.CompletedTask;
                            }

                            else if (!context.Request.Headers.ContainsKey("Authorization"))
                            {
                                context.Response.Headers.TryAdd("Auth-Fail-Type", ApplicationErrors.IdentityErrors.MissingToken.Code);
                                context.HandleResponse();
                                return Task.CompletedTask;
                            }
                            else
                            {
                                return Task.CompletedTask;
                            }

                        }
                        ,
                        OnForbidden = context =>
                        {

                            context.Response.Headers.TryAdd("Auth-Fail-Type", ApplicationErrors.IdentityErrors.ForbiddenAccess.Code);



                            return Task.CompletedTask;

                        }
                    };
                });

            services.AddAuthorization();

            return services;
        }
        public static IServiceCollection RegisterAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(o =>
            {
                o.AddMaps(Assembly.GetExecutingAssembly());
            });

            return services;
        }
        public static IServiceCollection RegisterDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection")!;


            if (string.IsNullOrWhiteSpace(connectionString))
                throw new MissingConfigurationSettingsException("DefaultConnection");


            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
                options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
            });

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            return services;
        }
        public static IServiceCollection RegisterFluentEmail(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddOptions<SmtpSettings>().Bind(configuration.GetSection(SmtpSettings.SectionName));

            var smtpSettings = configuration.GetSection(SmtpSettings.SectionName).Get<SmtpSettings>();

            if (smtpSettings is null)
                throw new MissingConfigurationSettingsException(SmtpSettings.SectionName);

            if (string.IsNullOrWhiteSpace(smtpSettings.SmtpHost))
                throw new MissingConfigurationSettingsException(nameof(smtpSettings.SmtpHost));

            if (string.IsNullOrWhiteSpace(smtpSettings.FromEmail))
                throw new MissingConfigurationSettingsException(nameof(smtpSettings.FromEmail));


            services.AddFluentEmail(smtpSettings!.FromEmail)
                    .AddRazorRenderer()
                    .AddSmtpSender(() => new SmtpClient(smtpSettings.SmtpHost, smtpSettings.SmtpPort)
                    {
                        EnableSsl = smtpSettings.UseSSL,
                        UseDefaultCredentials = false,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Credentials = smtpSettings.UseSSL ? new NetworkCredential(smtpSettings.FromEmail, smtpSettings.Password) : null
                    });


            return services;




        }
        public static IServiceCollection RegisterMemoryCache(this IServiceCollection services)
        {

            services.AddMemoryCache();

            return services;
        }
        public static IServiceCollection RegisterSignalR(this IServiceCollection services)
        {
            services.AddSignalR();
            return services;
        }
        public static IServiceCollection RegisterIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.Tokens.EmailConfirmationTokenProvider = "ResetPasswordOTPProvider";

            })
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders()
             .AddTokenProvider<ResetPasswordOTPTokenProvider<ApplicationUser>>("ResetPasswordOTPProvider");



            return services;
        }
        public static IServiceCollection RegisterRepositoriesAndUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            // Register Repositories here....

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();


            return services;
        }
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            //Register any additonal services here...
            services.AddTransient<ITokenService, TokenService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IDateTime, SettableDateProvider>();
            services.AddScoped<IFileManagerService, FileManagerService>();
            services.AddScoped<ApplicationDbContextInitializer>();


            return services;
        }
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {


            services.RegisterDbContext(configuration)
                    .RegisterFluentEmail(configuration)
                    .RegisterAutoMapper()
                    .RegisterRepositoriesAndUnitOfWork()
                    .RegisterServices()
                    .RegisterIdentity()
                    .AddJwtAuthentication(configuration)
                    .AddHangFireBackgroundJobWorker(configuration)
                    .RegisterSignalR()
                    .RegisterMemoryCache()
                    .AddLocalization();

            return services;
        }
        public class ResetPasswordOTPTokenProvider<T> : TotpSecurityStampBasedTokenProvider<T> where T : class
        {
            public override async Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<T> manager, T user)
            {
                return await manager.IsEmailConfirmedAsync(user) && !await manager.IsLockedOutAsync(user);
            }

            public override Task<string> GenerateAsync(string purpose, UserManager<T> manager, T user)
            {
                return base.GenerateAsync("ResetPasswordOTP:" + purpose, manager, user);
            }

            public override Task<string> GetUserModifierAsync(string purpose, UserManager<T> manager, T user)
            {
                return base.GetUserModifierAsync("ResetPasswordOTP:" + purpose, manager, user);
            }

            public override Task<bool> ValidateAsync(string purpose, string token, UserManager<T> manager, T user)
            {
                return base.ValidateAsync("ResetPasswordOTP:" + purpose, token, manager, user);
            }
        }
    }


}
