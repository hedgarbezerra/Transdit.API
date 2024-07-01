using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using System.Text.Json.Serialization;
using System.Text.Json;
using Transdit.API.Configuration;
using Transdit.Core.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Serilog;
using FluentValidation;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Transdit.Services.Validators;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Transdit.API.Configuration.BackgroundTasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

namespace Transdit.API
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var hostingEnvironment = builder.Environment;

            builder.Services.AddResponseCompression(op =>
            {
                op.EnableForHttps = true;
            });
            builder.Services.AddHttpContextAccessor();
            #region Configura Azure App Configurations
            var appConfigurationsConnectionString = builder.Configuration.GetConnectionString("AppConfig");
            builder.Configuration.AddAzureAppConfiguration((config) =>
            {
                config.Connect(appConfigurationsConnectionString)
                    .ConfigureRefresh(opt =>
                    {
                        opt.Register(AzureConfigurations.AZURE_CONFIG_CACHE_SENTINEL, true);
                        opt.SetCacheExpiration(AzureConfigurations.AZURE_CONFIG_CACHE_EXPIRACY);
                    });

                config.Select(KeyFilter.Any, LabelFilter.Null);
                config.Select(KeyFilter.Any, hostingEnvironment.EnvironmentName);
            });

            builder.Services.AddAzureAppConfiguration();
            #endregion

            #region Adiciona serviços
            builder.Services.AddGlobalization(builder);
            builder.Services.AddAPIVersioning();
            builder.Services.AddInternalSettings(builder.Configuration);
            builder.Services.AddInternalAuthencation(builder.Configuration);
            builder.Services.AddDatabaseConfiguration(builder.Configuration);
            builder.Services.AddInternalServices(builder.Configuration);

            var servicesAssembly = Assembly.GetAssembly(typeof(UserSignUpValidator));
            builder.Services.AddValidatorsFromAssembly(servicesAssembly);
            builder.Services.AddAutoMapper(servicesAssembly);

            builder.Services.AddHostedService<TemporaryFoldersCleanUp>();
            #endregion

            #region Define negociação de conteúdo
            builder.Services.AddControllers(op =>
            {
                op.RespectBrowserAcceptHeader = true;
                op.ReturnHttpNotAcceptable = true;
            })
                .AddXmlSerializerFormatters()
                .AddJsonOptions(ops =>
                {
                    ops.JsonSerializerOptions.PropertyNamingPolicy = null;
                    ops.JsonSerializerOptions.IgnoreReadOnlyProperties = false;
                    ops.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    ops.JsonSerializerOptions.WriteIndented = true;
                    ops.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    ops.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    ops.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    ops.JsonSerializerOptions.MaxDepth = 64;
                    ops.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });
            #endregion

            #region Setup logging mechanisms
            builder.AddLogging();
            builder.Host.UseSerilog();
            #endregion

            builder.Services.Configure<FormOptions>(options =>
            {
                var config = builder.Services.BuildServiceProvider().GetRequiredService<AppConfiguration>();

                options.MultipartBodyLengthLimit = config.FileSizeLimit;
            });

            var app = builder.Build();

            #region Setup middlewares 
            //app.UseTranscriptionUsageMiddleware();
            #endregion

            #region Setup Swagger UI
            IApiVersionDescriptionProvider apiVersioningProvider = builder.Services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var description in apiVersioningProvider?.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });
            #endregion

            #region Setup Globalization
            var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options?.Value);
            #endregion
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseCors();
            app.MapControllers();

            app.Run();
        }
    }
}