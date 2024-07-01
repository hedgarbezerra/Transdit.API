using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Web;
using Transdit.Core.Contracts;
using Transdit.Core.Domain;
using Transdit.Core.Models.Pagination;
using Transdit.Core.Models.Plans;
using Transdit.Core.Models.Users;
using Transdit.Services.Users;
using Transdit.Utilities.Extensions;

namespace Transdit.API.Controllers.V1
{
    [ExcludeFromCodeCoverage]
    [Authorize]
    [ApiVersion("1.0", Deprecated = false)]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ITranscriptionsService _transcriber;
        private readonly ICryptography _cryptography;
        private readonly IUsersService _userService;
        private readonly IPlansService _plansService;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUsersService userService, ICryptography cryptography, IPlansService plansService, IMapper mapper, ITranscriptionsService transcriber, ILogger<UsersController> logger)
        {
            _userService = userService;
            _cryptography = cryptography;
            _plansService = plansService;
            _mapper = mapper;
            _transcriber = transcriber;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] UserSignUp user)
        {
            try
            {
                var result = await _userService.Create(user);
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

        [HttpGet, Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> GetPaginated([FromQuery] PaginationInput pagination)
        {
            try
            {
                var route = HttpContext.Request.Path.Value;
                var users = await _userService.Get(pagination, route);
                if (users.TotalCount < 0)
                    return NoContent();

                var mappedUsers = _mapper.Map<PaginatedList<ApplicationUser>, PaginatedList<OutputUser>>(users);
                return Ok(mappedUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [Route("{email}")]
        [HttpGet, Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Get([FromRoute] string email)
        {
            try
            {
                var user = await _userService.Get(email);
                if (user is null)
                    return NoContent();

                var mapppedUser = _mapper.Map<ApplicationUser, OutputUser>(user);
                return Ok(mapppedUser);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserSignUp user)
        {
            try
            {
                var currentUserIdentity = User?.Identity?.Name ?? User?.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value;
                var result = await _userService.Update(user, currentUserIdentity);
                if (result.Successful)
                    return Ok();
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpPatch]
        [Route("email/change")]
        public async Task<IActionResult> EmailUpdate([FromBody] EmailUpdate newEmail)
        {
            try
            {
                var user = await GetCurrentUser();
                if (user is null)
                    return BadRequest();

                var result = await _userService.ChangeEmail(user, newEmail);
                if (!result.Successful)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpPatch]
        [Route("password/change")]
        public async Task<IActionResult> PasswordUpdate([FromBody] UserPasswordChange newPassword)
        {
            try
            {
                var user = await GetCurrentUser();
                if (user is null)
                    return BadRequest();

                var result = await _userService.ChangePassword(user, newPassword);
                if (!result.Successful)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }

        }

        [HttpGet]
        [Route("transcriptions")]
        public async Task<IActionResult> GetUsersTranscriptions([FromRoute] PaginationInput pagination)
        {
            try
            {
                var user = await GetCurrentUser();
                if (user is null)
                    return BadRequest();

                var route = HttpContext.Request.Path.Value;
                var transcriptions = _transcriber.Get(pagination, user, route);

                return Ok(transcriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("isactive")]
        public async Task<IActionResult> CheckUserPlanActivation()
        {
            try
            {
                var user = await GetCurrentUser();
                if (user is null)
                    return BadRequest();

                var userActivePlan = await _userService.IsUserPlanActive(user);

                return Ok(userActivePlan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("renew")]
        public async Task<IActionResult> RenewPlan()
        {//TODO: Parametrizar um período ou data 
            try
            {
                var user = await GetCurrentUser();
                if (user is null)
                    return BadRequest();

                _userService.RenewUserPlan(user);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }
        
        [HttpPatch]
        [Route("terms/accept")]
        public async Task<IActionResult> AgreeToServiceTerms()
        {
            try
            {
                var user = await GetCurrentUser();
                var result = await _userService.AcceptUserTerms(user);

                if (result)
                    return Ok();
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }


        [HttpGet, Authorize(Roles = "ADMINISTRATOR")]
        [Route("{username}/plan")]
        public async Task<IActionResult> GetUserPlan([FromRoute]string username)
        {
            try
            {
                var user = await _userService.Get(username);
                if (user is null)
                    return BadRequest($"Usuário com nome {username} não foi localizado");

                var plan = await _plansService.Get(user);
                if (plan is null)
                    return NoContent();
                var mappedPlan = _mapper.Map<ServicePlan, OutputPlan>(plan);
                return Ok(mappedPlan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("confirmation")]
        public async Task<IActionResult> EmailConfirmation([FromBody] EmailConfirmation confirmation)
        {
            try
            {
                var decodedEmail = confirmation.Email.HandleRequestEncodedParam();
                var decodedToken = confirmation.Token.HandleRequestEncodedParam();
                var decryptedEmail = _cryptography.Decrypt(decodedEmail);

                var result = await _userService.ConfirmUser(decryptedEmail, decodedToken);
                if (!result.Successful)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("password/reset")]
        public async Task<IActionResult> PasswordReset([FromBody] UserPasswordReset reset)
        {
            try
            {
                var decodedEmail = reset.Email.HandleRequestEncodedParam();
                reset.Email = _cryptography.Decrypt(decodedEmail);
                reset.Token = reset.Token.HandleRequestEncodedParam();

                var result = await _userService.ResetPassword(reset);
                if (!result.Successful)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("password/recovery/{email}")]
        public async Task<IActionResult> RecoverPassword([FromRoute] string email)
        {
            try
            {
                var recovery = await _userService.RecoverPassword(email);

                if (recovery.Successful)
                    return Ok(recovery);
                return BadRequest(recovery);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("component")]
        public async Task<IActionResult> GetPlan()
        {
            try
            {
                var user = await GetCurrentUser();
                var userPlan = _userService.GetUserPlan(user);
                var plan = await _plansService.Get(userPlan.RoleId.ToString());
                var transcriptions = _transcriber.Get(user);
                var consumedTime = transcriptions.Where(t => t.Date >= userPlan.Maturity.AddMonths(-1))
                    .Select(t => t.Usage)
                    .Aggregate(TimeSpan.Zero, (t1, t2) => t1 + t2);

                var data = new UserDataComponents()
                {
                    Name = user.Name,
                    UserName = user.UserName,
                    PlanName = plan.Name,
                    PlanIsActive = userPlan.IsActive,
                    PlanMaturity = userPlan.Maturity,
                    PlanAllowSaving = plan.AllowTranscriptionSaving,
                    TimeConsumed = consumedTime,
                    TimeAllowed = plan.MonthlyLimitUsage
                };
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }
        private async Task<ApplicationUser> GetCurrentUser()
        {
            var currentUserIdentity = User?.Identity?.Name ?? User?.Claims?.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value;
            var user = await _userService.Get(currentUserIdentity);
            return user;
        }
    }
}
