using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using Transdit.Core.Contracts;
using Transdit.Core.Domain;
using Transdit.Core.Models.Plans;
using Transdit.Core.Models.Users;

namespace Transdit.API.Controllers.V1
{
    [ExcludeFromCodeCoverage]
    [Authorize]
    [ApiVersion("1.0", Deprecated = false)]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PlansController : ControllerBase
    {
        private readonly IPlansService _plansService;
        private readonly IMapper _mapper;
        private readonly ILogger<PlansController> _logger;

        public PlansController(IPlansService plansService, IMapper mapper, ILogger<PlansController> logger)
        {
            _plansService = plansService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            try
            {
                var plans = _plansService.Get();

                if (!plans.Any())
                    return NoContent();

                var mappedPlans = _mapper.Map<IEnumerable<ServicePlan>, IEnumerable<OutputPlan>>(plans);
                return Ok(mappedPlans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet, Authorize(Roles = "ADMINISTRATOR")]
        [Route("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            try
            {
                var plan = await _plansService.Get(id);
                if (plan is null)
                    return NoContent();

                var mappedPlan = _mapper.Map<OutputPlan>(plan);
                return Ok(mappedPlan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpGet, Authorize(Roles = "ADMINISTRATOR")]
        [Route("{id}/users")]
        public async Task<IActionResult> GetUsers([FromRoute]string id)
        {
            try
            {
                var plan = await _plansService.Get(id);
                if (plan is null)
                    return BadRequest($"Nenhum plano com identificador {id} foi localizado");

                var usersInPlan = await _plansService.GetUsersInPlan(plan);
                if (!usersInPlan.Any())
                    return NoContent();

                var usersMapped = _mapper.Map<ICollection<ApplicationUser>, ICollection<OutputUser>>(usersInPlan);
                return Ok(usersMapped);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }
    }
}
