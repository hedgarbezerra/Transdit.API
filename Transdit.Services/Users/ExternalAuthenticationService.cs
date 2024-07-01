using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Constants;
using Transdit.Core.Models.Users;

namespace Transdit.Services.Users
{
    public interface IExternalAuthenticationService
    {
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(SSOExternal externalAuth);
    }

    public class ExternalAuthenticationService : IExternalAuthenticationService
    {
        private readonly GoogleSettings _googleSettings;

        public ExternalAuthenticationService(GoogleSettings googleSettings)
        {
            _googleSettings = googleSettings;
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(SSOExternal externalAuth)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _googleSettings.ClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.idToken, settings);
            return payload;
        }
    }
}
