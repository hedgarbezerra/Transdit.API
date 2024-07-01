using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using Transdit.Core.Constants;

namespace Transdit.API.Controllers.V1
{
    [ExcludeFromCodeCoverage]
    [ApiVersion("1.0", Deprecated = false)]
    [Route("api/app")]
    [Route("api/v{version:apiVersion}/app")]
    public class ConfigurationController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("languages")]
        public IActionResult LocalizableLanguages() => Ok(AppConfiguration.LocalizableLanguages);
    }
}
