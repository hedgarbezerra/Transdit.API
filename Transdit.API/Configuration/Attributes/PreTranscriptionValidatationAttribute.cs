using Google.Protobuf;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Transdit.Core.Contracts;
using Transdit.Core.Models;

namespace Transdit.API.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class PreTranscriptionValidatationAttribute : Attribute, IAsyncActionFilter
    {
        private readonly ILogger<PreTranscriptionValidatationAttribute> _logger;
        private readonly IUsersService _userService;
        private readonly IPlansService _plansService;
        private readonly ITranscriptionsService _transcriptionsService;

        public PreTranscriptionValidatationAttribute(ILogger<PreTranscriptionValidatationAttribute> logger, IUsersService userService, IPlansService plansService, ITranscriptionsService transcriptionsService)
        {
            _logger = logger;
            _userService = userService;
            _plansService = plansService;
            _transcriptionsService = transcriptionsService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                var userPrincipal = context.HttpContext.User;
                var currentUserIdentity = userPrincipal?.Identity?.Name ?? userPrincipal?.Claims?.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value;
                var user = await _userService.Get(currentUserIdentity);
                var transcriptions = _transcriptionsService.Get(user);
                var userPlan = _userService.GetUserPlan(user);

                if (userPlan is null || !userPlan.IsActive || userPlan.Maturity < DateTime.Now)
                {
                    context.Result = new BadRequestObjectResult(new PlanValidationResult(false, $"Plano está inativo ou vencido."));
                    return;
                }

                var totalMinutesFromLastMonth = transcriptions
                    .Where(t => t.Date >= userPlan.Maturity.AddMonths(-1))
                    .Sum(t => t.Usage.TotalMinutes);
                var plan = await _plansService.Get(user);

                if (totalMinutesFromLastMonth > plan.MonthlyLimitUsage.TotalMinutes)
                {
                    context.Result = new BadRequestObjectResult(new PlanValidationResult(false, $"Consumo mensal está esgotado."));
                    return;
                }

                await next();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Houve um erro ao executar middleware de transcrições: {ex.Message}", ex.StackTrace);
                await next();
            }
        }
    }
    public class PlanValidationResult : DefaultControllerResponse<object>
    {
        public PlanValidationResult(bool success, string message)
        {
            Successful = success;
            Messages.Add(message);
        }
    }
}
