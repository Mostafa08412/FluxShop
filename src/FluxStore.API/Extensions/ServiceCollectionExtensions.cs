using Asp.Versioning;
using Bogus;
using FastEndpoints;
using FluentValidation;
using FluxStore.Api.Domain.Enums;
using FluxStore.Api.Domain.UserAggregate;
using FluxStore.Api.Extensions;
using FluxStore.Api.Infrastructure.Persistence;
using FluxStore.Api.Infrastructure.Services;
using FluxStore.Api.Shared.Abstractions;
using FluxStore.Api.Shared.Behaviors;
using FluxStore.Api.Shared.Errors;
using FluxStore.Api.Shared.Settings;
using Hangfire;
using Hangfire.SqlServer;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;

namespace FluxStore.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddFluxStoreDatabaseContext(configuration)
                    .AddIdentity()
                    .AddMediatRCore()
                    .AddValidationPipeline()
                    .AddTransactionPipeline()
                    .AddFastEndpoint()
                    .AddCaching()
                    .AddSwaggerDocumentation()
                    .AddFluentEmail(configuration)
                    .AddJwtAuthentication(configuration)
                    .AddHangFireBackgroundJobWorker(configuration)
                    .AddCorsSettings(configuration)
                    .AddOptions(configuration)
                    .AddServices(configuration);

            return services;
        }
        private static IServiceCollection AddCorsSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var corsSettings = configuration.GetSection(CorsSettings.SectionName).Get<CorsSettings>();

            if (corsSettings is not null)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(corsSettings.PolicyName, builder =>
                    {
                        builder.WithOrigins(corsSettings.AllowedOrigins)
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                        if (corsSettings.AllowCredentials)
                        {
                            builder.AllowCredentials();
                        }
                    });
                });
            }
            return services;
        }
        private static IServiceCollection AddCaching(this IServiceCollection services)
        {

            services.AddHybridCache(options =>
            {
                options.MaximumPayloadBytes = 1024 * 1024;
                options.MaximumKeyLength = 1024;
                options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(5),
                    LocalCacheExpiration = TimeSpan.FromMinutes(5)
                };
            });
            return services;
        }
        private static IServiceCollection AddHangFireBackgroundJobWorker(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddHangfire(X => X.UseSqlServerStorage(connectionString, new SqlServerStorageOptions
            {
                PrepareSchemaIfNecessary = true

            }));


            services.AddHangfireServer();

            return services;
        }
        private static IServiceCollection AddFastEndpoint(this IServiceCollection services)
        {
            services
           .AddFastEndpoints()
           .AddApiVersioning(
           c =>
           {
               c.ApiVersionReader = new UrlSegmentApiVersionReader();
               c.DefaultApiVersion = new ApiVersion(1, 0);
               c.AssumeDefaultVersionWhenUnspecified = true;
               c.UnsupportedApiVersionStatusCode = StatusCodes.Status400BadRequest;
           }
           );
            return services;
        }
        private static IServiceCollection AddFluentEmail(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<SmtpSettings>().Bind(configuration.GetSection(SmtpSettings.SectionName));

            services.AddScoped<IEmailService, EmailService>();

            var smtpSettings = configuration.GetSection(SmtpSettings.SectionName).Get<SmtpSettings>();

            services
              .AddFluentEmail(smtpSettings!.User, smtpSettings!.Name)
              .AddRazorRenderer()

              .AddSmtpSender(() => new SmtpClient(smtpSettings.Server, smtpSettings.Port)
              {
                  EnableSsl = smtpSettings.UseSsl,
                  UseDefaultCredentials = false,
                  DeliveryMethod = SmtpDeliveryMethod.Network,
                  Credentials = smtpSettings.RequiresAuthentication ? new NetworkCredential(smtpSettings.User, smtpSettings.Password) : null
              });

            return services;
        }
        private static IServiceCollection AddFluxStoreDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection")!;
            var databaseSetupSettings = configuration.GetSection(DatabaseSetupSettings.SectionName).Get<DatabaseSetupSettings>()!;
            services.AddDbContext<ApplicationDbContext>(
                opt =>
                {
                    opt.UseSqlServer(connectionString);
                    opt.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

                    if (databaseSetupSettings.SeedInitialData)
                    {

                        opt.UseAsyncSeeding(async (context, _, ct) =>
                        {
                            Log.Information("[[[[ Seeding initial data into the database as per configuration.]]]]");

                            var roles = new string[]{
                            nameof(ApplicationRoles.Admin),
                            nameof(ApplicationRoles.User)};
                            var identityRoles = new List<IdentityRole<Guid>>();

                            if (await context.Set<IdentityRole<Guid>>().AnyAsync(ct) == false)
                            {
                                foreach (var role in roles)
                                {
                                    var identityRole = new IdentityRole<Guid> { Name = role, NormalizedName = role.ToUpper() };
                                    identityRoles.Add(identityRole);
                                    context.Set<IdentityRole<Guid>>().Add(identityRole);
                                }
                                await context.SaveChangesAsync(ct);
                            }


                            if (!(await context.Set<ApplicationUser>().AnyAsync(ct)))
                            {
                                var faker = new Faker<ApplicationUser>()
                                  .UseSeed(420)
                                  .RuleFor(u => u.UserName, f => f.Internet.UserName())
                                  .RuleFor(u => u.Email, f => f.Internet.Email())
                                  .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                                  .RuleFor(u => u.LastName, f => f.Name.LastName())
                                  .RuleFor(u => u.NormalizedEmail, (u, f) => f.Email!.ToUpper())
                                  .RuleFor(u => u.NormalizedUserName, (u, f) => f.UserName!.ToUpper())
                                  .RuleFor(u => u.PasswordHash, (u, f) => new PasswordHasher<ApplicationUser>().HashPassword(f, "user@localhost"))
                                  .RuleFor(u => u.ConcurrencyStamp, (u, _) => Guid.NewGuid().ToString())
                                  .RuleFor(u => u.SecurityStamp, (u, _) => Guid.NewGuid().ToString())
                                  .RuleFor(u => u.EmailConfirmed, f => true);


                                var appUsers = faker.Generate(10);

                                // Add Application Users

                                await context.Set<ApplicationUser>().AddRangeAsync(appUsers);

                                // Add Identity User Roles
                                await context.Set<IdentityUserRole<Guid>>().AddRangeAsync(appUsers.Select(x => new IdentityUserRole<Guid>
                                {
                                    RoleId = identityRoles.Shuffle().First().Id,
                                    UserId = x.Id
                                }));

                                // Add Domain Users

                                await context.Set<User>().AddRangeAsync(appUsers.Select(X => User.Create(X.Id, X.FirstName, X.LastName, X.UserName!, X.Email!).Value));



                            }

                            await context.SaveChangesAsync(ct);



                            Log.Information("[[[[ Database seeding completed..]]]]");


                        });


                    }

                });

            return services;
        }
        private static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(op =>
            {
                op.Password.RequireDigit = false;
                op.Password.RequireNonAlphanumeric = true;
                op.Password.RequireUppercase = false;
                op.Password.RequiredLength = 6;
                op.Password.RequireLowercase = true;

                op.User.RequireUniqueEmail = true;

                op.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                op.Lockout.MaxFailedAccessAttempts = 5;

                op.Tokens.EmailConfirmationTokenProvider = "ResetPasswordOTPProvider";

            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<ResetPasswordOTPTokenProvider<ApplicationUser>>("ResetPasswordOTPProvider");

            return services;

        }
        private class ResetPasswordOTPTokenProvider<T> : TotpSecurityStampBasedTokenProvider<T> where T : class
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
        private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLocalization();
            services.AddHttpContextAccessor();

            services.AddScoped<CurrentUser>();
            services.AddTransient<IDateTime, DateTimeProvider>();

            services.AddScoped<TokenService>();

            return services;
        }
        private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<TokenSettings>().Bind(configuration.GetSection(TokenSettings.SectionName));
            services.AddOptions<ExternalAuthenticationSettings>().Bind(configuration.GetSection(ExternalAuthenticationSettings.SectionName));
            return services;

        }
        private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAuthorizationBuilder();

            services.AddOptions<TokenSettings>().Bind(configuration.GetSection(TokenSettings.SectionName));



            var section = configuration.GetSection(TokenSettings.SectionName);

            var tokenSettings = section.Get<TokenSettings>();





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
                                context.Response.Headers.TryAdd("Auth-Fail-Type", IdentityErrors.InvalidToken.Code);
                                context.HandleResponse();
                                return Task.CompletedTask;
                            }


                            else if (context.AuthenticateFailure is SecurityTokenExpiredException)
                            {
                                context.Response.Headers.TryAdd("Auth-Fail-Type", IdentityErrors.ExpiredToken.Code);
                                context.HandleResponse();
                                return Task.CompletedTask;
                            }

                            else if (!context.Request.Headers.ContainsKey("Authorization"))
                            {
                                context.Response.Headers.TryAdd("Auth-Fail-Type", IdentityErrors.MissingToken.Code);
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

                            context.Response.Headers.TryAdd("Auth-Fail-Type", IdentityErrors.ForbiddenAccess.Code);



                            return Task.CompletedTask;

                        }
                    };
                });

            services.AddAuthorization();

            return services;
        }
        private static IServiceCollection AddMediatRCore(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
            return services;
        }
        public static IServiceCollection AddValidationPipeline(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
        private static IServiceCollection AddTransactionPipeline(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            return services;
        }
        private static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FluxShop E-Commerce Platform.",
                    Version = "v1",
                    Description = "This is documentation for FluxShop E-Commerce Platform Api Version 1.0"

                });

                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "FluxShop E-Commerce Platform.",
                    Version = "v2",
                    Description = "This is documentation for FluxShop E-Commerce Platform Api Version 2.0"
                });

                // Include endpoints in a swagger doc based on the URL segment version in the route
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (string.IsNullOrEmpty(apiDesc.RelativePath)) return false;
                    var relativePath = apiDesc.RelativePath!.ToLowerInvariant();
                    // docName is like "v1" or "v2". Match "/v1/" or "/v2/" in the path
                    return relativePath.Contains($"/v{docName.Substring(1)}/");
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "Json Web Token",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
            });

            return services;
        }


    }
}
