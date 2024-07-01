using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using Transdit.Core.Contracts;
using Transdit.Core.Models.Users;
using Transdit.Services.Users;

namespace Transdit.API.Controllers.V1
{
    [ExcludeFromCodeCoverage]
    [ApiVersion("1.0", Deprecated = false)]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ICryptography _cryptography;
        private readonly IExternalAuthenticationService _externalAuthService;
        private readonly IAuthenticationService _authService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IAuthenticationService authService, ICryptography cryptography, ILogger<AuthenticationController> logger, IExternalAuthenticationService externalAuthService)
        {
            _authService = authService;
            _cryptography = cryptography;
            _logger = logger;
            _externalAuthService = externalAuthService;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] Login login)
        {
            try
            {
                var result = await _authService.Authenticate(login);

                if (result.Successful)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("sso")]
        public async Task<IActionResult> AuthenticateGoogle([FromBody] SSOExternal login)
        {
            try
            {
                switch (login.provider)
                {
                    case "MICROSOFT":
                        _externalAuthService.VerifyGoogleToken(login);
                        break;
                    case "GOOGLE":
                        break;
                    default:
                        break;
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("validate")]
        public IActionResult ValidateTokenQuery([FromQuery] string token)
        {
            try
            {
                var result = _authService.ValidateToken(token);

                if (result)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("encrypt")]
        public IActionResult Encrypt([FromQuery] string value)
        {
            try
            {
                var encryptedValue = _cryptography.Encrypt(value);
                return Ok(encryptedValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("decrypt")]
        public IActionResult Decrypt([FromQuery] string value)
        {
            try
            {
                var decryptedValue = _cryptography.Decrypt(value);
                return Ok(decryptedValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }
    }
}
