using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Domain;

namespace Transdit.Core.Models.Users
{
    public class AuthenticationResult : DefaultControllerResponse<JwtTokenResponse>
    {
    }

    public class JwtTokenResponse
    {
        public JwtTokenResponse()
        {            
        }
        public JwtTokenResponse(string token)
        {
            Token = token;
        }
        public JwtTokenResponse(string token, DateTimeOffset? expiracy)
        {
            Token = token;
            Expiracy = expiracy;
        }

        public string Token { get; set; } = string.Empty;
        public DateTimeOffset? Expiracy { get; set; }
        public bool IsValid { get { return Expiracy.HasValue ? DateTime.UtcNow < Expiracy.Value : false; } }
    }
}
