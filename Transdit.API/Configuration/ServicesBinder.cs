using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Transdit.API.Configuration.Versioning;
using Transdit.Core.Constants;
using Serilog;
using System.Text;
using Transdit.API.Configuration.Policies;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Transdit.Core.Domain;
using Transdit.Repository;
using Transdit.API.Configuration.Database;
using Microsoft.EntityFrameworkCore;
using Transdit.Core.Contracts;
using Transdit.Services.Common;
using Transdit.Core.Builders;
using Transdit.Core.Models.Notification;
using Transdit.Repository.Repositories;
using Transdit.Core.Models;
using Transdit.Services.Users;
using Transdit.Services.Emails;
using Microsoft.AspNetCore.Authorization;
using Transdit.Utilities.Security;
using System.Diagnostics.CodeAnalysis;
using Transdit.Services.Logs;
using Transdit.Services.Roles;
using Transdit.Services.Transcriptions;
using Transdit.Core.Models.Enums;
using Transdit.Services.Common.Convertion;
using Transdit.API.Configuration.Attributes;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Transdit.Utilities.Globalization;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;

namespace Transdit.API.Configuration
{
    [ExcludeFromCodeCoverage]
    internal static class ServicesBinder
    {
        /// <summary>
        /// Adiciona funcionalidade local de tradução de strings utilizando o IStringLocalizer
        /// </summary>
        /// <param name="services"></param>
        internal static void AddGlobalization(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = AppConfiguration.LocalizableLanguages.ToArray();
                options.SetDefaultCulture("pt-BR")
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures);

                options.ApplyCurrentCultureToResponseHeaders = true;
            });

        }
        /// <summary>
        /// Adiciona documentação da versão da API pelo Swagger/OpenAPI
        /// </summary>
        /// <param name="services">coleção de serviços no momento de construção</param>
        internal static void AddAPIVersioning(this IServiceCollection services)
        {
            #region Versionamento da API (Considerada pelo header ou pela query string)
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("x-api-version"), new QueryStringApiVersionReader("api-version"));
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
            });
            #endregion

            #region Swagger documentation setup
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerConfiguration>();
            });
            #endregion
        }

        /// <summary>
        /// Inicializa e adiciona classes de configurações como singleton
        /// </summary>
        /// <param name="services">coleção de serviços no momento de construção</param>
        /// <param name="configurations">configurações da aplicação em construção</param>
        internal static void AddInternalSettings(this IServiceCollection services, ConfigurationManager configurations)
        {
            var smtpSettings = new SmtpSettings();
            new ConfigureFromConfigurationOptions<SmtpSettings>(
                configurations.GetSection("SmtpSettings"))
                    .Configure(smtpSettings);

            services.AddSingleton(smtpSettings);

            var tokenSettings = new JwtSettings();
            new ConfigureFromConfigurationOptions<JwtSettings>(
                configurations.GetSection("JwtTokenSetting"))
                    .Configure(tokenSettings);

            services.AddSingleton(tokenSettings);

            var appConfigs = new AppConfiguration();
            new ConfigureFromConfigurationOptions<AppConfiguration>(configurations.GetSection("AppConfigurations"))
                .Configure(appConfigs);
            services.AddSingleton(appConfigs);

            var bucketSettings = new GoogleSettings();
            new ConfigureFromConfigurationOptions<GoogleSettings>(configurations.GetSection("Google"))
                .Configure(bucketSettings);
            services.AddSingleton(bucketSettings);
        }

        /// <summary>
        /// Adiciona log automático como middleware com Serilog
        /// </summary>
        /// <param name="builder">Construtor da aplicação web</param>
        internal static void AddLogging(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration, "Serilog")
                .CreateLogger();
        }

        /// <summary>
        /// Inicializa autenticação e configura suporte ao JWT
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        internal static void AddInternalAuthencation(this IServiceCollection services, IConfiguration configurations)
        {
            var tokenSettings = services.BuildServiceProvider().GetRequiredService<JwtSettings>();
            byte[] tokenKeyBytes = Encoding.ASCII.GetBytes(tokenSettings.Key);
            if (tokenKeyBytes is not null && tokenKeyBytes.Length > 0)
            {
                services.AddAuthentication(opt =>
                {
                    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

                }).AddJwtBearer(opt =>
                {
                    opt.RequireHttpsMetadata = false;
                    opt.SaveToken = true;
                    opt.IncludeErrorDetails = true;
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(tokenKeyBytes),
                        ValidateIssuer = true,
                        ValidIssuer = tokenSettings.Issuer,
                        ValidateAudience = false,
                        ValidateTokenReplay = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
                services.AddAuthorization(opt => opt.AddAuthorizationPolicies());
                services.AddCors(options => options.AddCorsPolicies(configurations));
            }
        }

        /// <summary>
        /// Adiciona configurações do Identity com Entity Framework
        /// </summary>
        /// <param name="services">coleção de serviços no momento de construção</param>
        /// <param name="configurations">configurações da aplicação em construção</param>
        internal static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var identityDbConnectionString = configuration.GetConnectionString("SQLDatabase");
            ArgumentNullException.ThrowIfNull(identityDbConnectionString, nameof(identityDbConnectionString));

            services.AddDbContext<SqlIdentityContext>(opt => opt.UseSqlServer(identityDbConnectionString))
                .AddIdentity<ApplicationUser, ServicePlan>(op =>
                {
                    IdentityConfigurationOptions.Configure(op);
                })
                .AddEntityFrameworkStores<SqlIdentityContext>()
                .AddDefaultTokenProviders();
        }

        /// <summary>
        /// Adiciona serviços internos no Service provider do .NET
        /// </summary>
        /// <param name="services">coleção de serviços no momento de construção</param>
        /// <param name="configurations">configurações da aplicação em construção</param>
        internal static void AddInternalServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IMailNotificationBuilder, MailNotificationBuilder>();

            services.AddTransient<IRepository<LogItem>, LogRepository>();
            services.AddTransient<IRepository<ApplicationUserPlan>, UserPlansRepository>();
            services.AddTransient<IRepository<Transcription>, TranscriptionsRepository>();
            services.AddTransient<IRepository<CustomDictionary>, CustomDictionaryRepository>();

            services.AddScoped<ICustomDictionariesService, CustomDictionariesService>();
            services.AddScoped<IPlansService, PlansService>();
            services.AddScoped<ILoggingService, LoggingService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<INotificator<EmailNotification>, MailNotificator>();
            services.AddScoped<IHttpConsumer, HttpConsumer>();
            services.AddScoped<IYoutubeDownloader, YoutubeDownloader>();
            services.AddSingleton<ITranscriptionMediaConvertion>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<TranscriptionMediaConvertion>>();
                var env = sp.GetRequiredService<IWebHostEnvironment>();
                return new TranscriptionMediaConvertion(env.WebRootPath, logger);
            });
            services.AddSingleton<IEmailFromFileGenerator, EmailFromFileGenerator>(sv =>
            {
                var env = sv.GetRequiredService<IWebHostEnvironment>();
                return new EmailFromFileGenerator(env.WebRootPath);
            });
            services.AddSingleton<IFileConverter, ExportFileConvertion>(sv =>
            {
                var env = sv.GetRequiredService<IWebHostEnvironment>();
                var htmlFromFile = sv.GetRequiredService<IEmailFromFileGenerator>();

                return new ExportFileConvertion(env.WebRootPath, htmlFromFile);
            });
            services.AddSingleton<ICryptography, Cryptography>();
            services.AddSingleton<IUriService>(o =>
            {
                var accessor = o.GetRequiredService<IHttpContextAccessor>();
                var request = accessor?.HttpContext?.Request;
                var uri = string.Concat(request?.Scheme, "://", request?.Host.ToUriComponent());

                return new UriService(uri);
            });            
            services.AddScoped<IWelcomeEmailSender>(o =>
            {
                var emailGen = o.GetRequiredService<IEmailFromFileGenerator>();
                var userManager = o.GetRequiredService<UserManager<ApplicationUser>>();
                var notificator = o.GetRequiredService<INotificator<AzureEmailNotification>>();
                var notificatorBuilder = o.GetRequiredService<IMailNotificationBuilder>();
                var crypto = o.GetRequiredService<ICryptography>();
                var configuration = o.GetRequiredService<AppConfiguration>();

                return new WelcomeEmailSender(emailGen, userManager, notificator, notificatorBuilder, configuration.WebAppUrl, crypto);
            });
            services.AddScoped<IPasswordRecoveryEmailSender>(o =>
            {
                var emailGen = o.GetRequiredService<IEmailFromFileGenerator>();
                var userManager = o.GetRequiredService<UserManager<ApplicationUser>>();
                var notificator = o.GetRequiredService<INotificator<AzureEmailNotification>>();
                var crypto = o.GetRequiredService<ICryptography>();
                var accessor = o.GetRequiredService<IHttpContextAccessor>();
                var configuration = o.GetRequiredService<AppConfiguration>();

                return new PasswordRecoveryEmailSender(emailGen, userManager, notificator, configuration.WebAppUrl, crypto);
            });
            services.AddScoped<IExternalAuthenticationService, ExternalAuthenticationService>();
            services.AddScoped<IGoogleSpeech>(sp =>
            {
                var settings = sp.GetRequiredService<GoogleSettings>();
                var logger = sp.GetRequiredService<ILogger<GoogleSpeech>>();
                var mediaConverter = sp.GetRequiredService<ITranscriptionMediaConvertion>();
                var stringLocalizer = sp.GetRequiredService<IStringLocalizer<Languages>>();

                return new GoogleSpeech(settings.Key, logger, mediaConverter, stringLocalizer);
            });
            services.AddScoped<IGoogleBucket>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<GoogleBucketClient>>();
                var settings = sp.GetRequiredService<GoogleSettings>();

                return new GoogleBucketClient(settings, logger);
            });
            services.AddScoped<ITranscriptionsService,  TranscriptionsService>();
            services.AddScoped<INotificator<AzureEmailNotification>>(sp =>
            {
                var communicationServicesConnectionString = configuration.GetConnectionString("ComsServices");
                var logger = sp.GetRequiredService<ILogger<AzureEmailNotificator>>();

                return new AzureEmailNotificator(communicationServicesConnectionString, logger);
            });
            services.AddScoped<PreTranscriptionValidatationAttribute>();
        }
    }
}