using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Transdit.API.Configuration.Policies
{
    internal static class AuthorizationPolicies
    {
        public static void AddAuthorizationPolicies(this AuthorizationOptions authorizationOptions)
        {
            authorizationOptions.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                    .RequireAuthenticatedUser().Build();

            authorizationOptions.AddPolicy("ADMINISTRATOR", pol =>
            {
                pol.AddAuthenticationSchemes("Bearer")
                .RequireAuthenticatedUser()
                .RequireRole("ADMINISTRATOR");
            });
        }
    }
}
