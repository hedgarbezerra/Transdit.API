using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Constants
{
    [ExcludeFromCodeCoverage]
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string ExpirationInMinutes { get; set; } = string.Empty;
        public TimeSpan Validity
        {
            get
            {
                if (string.IsNullOrEmpty(ExpirationInMinutes))
                    return TimeSpan.Zero;
                var expiration = double.Parse(ExpirationInMinutes);
                return TimeSpan.FromMinutes(expiration);
            }
        }
    }
}
