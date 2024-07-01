using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Constants
{
    [ExcludeFromCodeCoverage]
    public  class AzureConfigurations
    {
        public static TimeSpan AZURE_CONFIG_CACHE_EXPIRACY = TimeSpan.FromMinutes(1);
        public static string AZURE_CONFIG_CACHE_SENTINEL = "Settings:CacheRefresh";
    }
}
