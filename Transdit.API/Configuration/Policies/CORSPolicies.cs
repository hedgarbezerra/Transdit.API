using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Transdit.API.Configuration.Policies
{
    internal static class CORSPolicies
    {
        public static void AddCorsPolicies(this CorsOptions corsOptions, IConfiguration configuration)
        {
            corsOptions.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins(@"https://transdit.com.br", @"https://transdit-app.azurewebsites.net", configuration["AppConfigurations:WebAppUrl"]);
            });
        }
    }
}
