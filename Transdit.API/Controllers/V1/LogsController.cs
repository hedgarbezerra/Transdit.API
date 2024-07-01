using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using Transdit.Core.Contracts;
using Transdit.Core.Models.Pagination;

namespace Transdit.API.Controllers.V1
{
    [ExcludeFromCodeCoverage]
    [ApiVersion("1.0", Deprecated = false)]
    [Authorize]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly ILoggingService _loggingService;
        private readonly ILogger<LogsController> _logger;

        public LogsController(ILoggingService loggingService, ILogger<LogsController> logger)
        {
            _loggingService = loggingService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetPaginated([FromQuery] PaginationInput pagination)
        {
            try
            {
                var route = HttpContext.Request.Path.Value;
                var result = _loggingService.Get(pagination, route);

                if (result.TotalCount > 0)
                    return Ok(result);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar transcrição", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Houve um erro ao processar sua solicitação, tente novamente em instantes.");
            }
        }

        [HttpGet]
        [Route("between")]
        public IActionResult GetInRange([FromQuery] DateTime begin, DateTime end)
        {
            try
            {
                var result = _loggingService.Get(begin, end);

                if (result.Count() > 0)
                    return Ok(result);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar transcrição", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Houve um erro ao processar sua solicitação, tente novamente em instantes.");
            }
        }
    }
}
