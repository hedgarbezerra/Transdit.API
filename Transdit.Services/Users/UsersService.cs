using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Contracts;
using Transdit.Core.Domain;
using Transdit.Core.Models;
using Transdit.Core.Models.Notification;
using Transdit.Core.Models.Pagination;
using Transdit.Core.Models.Users;
using Transdit.Repository.Repositories;
using Transdit.Services.Common;
using Transdit.Services.Emails;
using Transdit.Utilities.Extensions;

namespace Transdit.Services.Users
{
    public class UsersService : IUsersService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ServicePlan> _roleManager;
        private readonly IValidator<UserSignUp> _signUpValidator;
        private readonly IValidator<UserPasswordChange> _passwordChangeValidator;
        private readonly IValidator<UserPasswordReset> _passwordResetValidator;
        private readonly IValidator<EmailUpdate> _emailUpdateValidator;
        private readonly IUriService _uriService;
        private readonly IRepository<ApplicationUserPlan> _usersPlansRepository;
        private readonly IPasswordRecoveryEmailSender _passwordRecovery;
        private readonly IWelcomeEmailSender _welcomeEmail;
        private readonly IMapper _mapper;

        public UsersService(UserManager<ApplicationUser> userManager, IValidator<UserSignUp> signUpValidator, IUriService uriService,
            RoleManager<ServicePlan> roleManager, IValidator<UserPasswordChange> passwordChangeValidator, IRepository<ApplicationUserPlan> usersPlansRepository,
            IPasswordRecoveryEmailSender passwordRecovery, IWelcomeEmailSender welcomeEmail, IValidator<EmailUpdate> emailUpdateValidator, IValidator<UserPasswordReset> passwordResetValidator, IMapper mapper)
        {
            _userManager = userManager;
            _signUpValidator = signUpValidator;
            _uriService = uriService;
            _roleManager = roleManager;
            _passwordChangeValidator = passwordChangeValidator;
            _usersPlansRepository = usersPlansRepository;
            _passwordRecovery = passwordRecovery;
            _welcomeEmail = welcomeEmail;
            _emailUpdateValidator = emailUpdateValidator;
            _passwordResetValidator = passwordResetValidator;
            _mapper = mapper;
        }

        public async Task<ApplicationUser> Get(string username) => await _userManager.FindByEmailAsync(username) ?? await _userManager.FindByNameAsync(username);
        public async Task<ApplicationUser> Get(int id) => await _userManager.FindByIdAsync(id.ToString());
        public async Task<PaginatedList<ApplicationUser>> Get(PaginationInput pagination, string route)
        {
            var usersQuery = string.IsNullOrEmpty(pagination.SearchTerm) ?
                _userManager.Users :
                _userManager.Users.Where(u => u.UserName.Contains(pagination.SearchTerm, StringComparison.InvariantCultureIgnoreCase) ||
                    u.Email.Contains(pagination.SearchTerm, StringComparison.InvariantCultureIgnoreCase));
            var paginatedUsers = new PaginatedList<ApplicationUser>(usersQuery, _uriService, route, pagination.Index, pagination.Size);

            return paginatedUsers;
        }
        public async Task<UserOperationResult> Create(UserSignUp user)
        {
            var result = new UserOperationResult();
            var validationResult = _signUpValidator.Validate(user);
            if (validationResult.IsValid)
            {
                var appUser = new ApplicationUser() { Email = user.Email, BirthDate = user.BirthDate, UserName = user.UserName, Name = user.Name, TermsAgreed = user.TermsAccepted};
                var resultUser = await _userManager.CreateAsync(appUser, user.Password);
                if (resultUser.Succeeded)
                {
                    var plan = await _roleManager.FindByIdAsync(user.PlanId.ToString());
                    if (plan != null)
                    {
                        await AddToPlan(appUser, plan);
                        var newClaims = await GetClaims(appUser, true, plan.NormalizedName);
                        var resultClaims = await _userManager.AddClaimsAsync(appUser, newClaims);

                        if (!resultClaims.Succeeded)
                            result.Messages.AddIdentityErrors(resultClaims.Errors);

                        var mappedUser = _mapper.Map<OutputUser>(appUser);
                        result.Data = mappedUser;
                        await _welcomeEmail.Send(appUser, plan);

                        result.Successful = await IsUserPlanActive(appUser);
                    }
                    else
                    {
                        _userManager.DeleteAsync(appUser);
                        result.Messages.Add("Plano não identificado.");
                    }
                }
                else
                    result.Messages.AddIdentityErrors(resultUser.Errors);
            }
            else
                result.Messages.AddValidationErrors(validationResult.Errors);
            return result;
        }
        public async Task<UserOperationResult> Update(UserSignUp user, string currentUser)
        {
            var result = new UserOperationResult();
            var validationResult = _signUpValidator.Validate(user);
            if (validationResult.IsValid)
            {
                var appUser = await Get(currentUser);
                if (appUser is null)
                {
                    result.Messages.Add($"User by name {user.UserName} not found.");
                    return result;
                }

                UpdateApplicationUserFields(appUser, user);
                var resultUpdate = await _userManager.UpdateAsync(appUser);
                if (resultUpdate.Succeeded)
                {
                    var role = await _roleManager.FindByIdAsync(user.PlanId.ToString());
                    await AddToPlan(appUser, role);

                    var oldClaims = await _userManager.GetClaimsAsync(appUser);
                    var newClaims = await GetClaims(appUser, true, role.NormalizedName);
                    var resultRemoveClaims = await _userManager.RemoveClaimsAsync(appUser, oldClaims);
                    if (!resultRemoveClaims.Succeeded)
                        result.Messages.AddIdentityErrors(resultRemoveClaims.Errors);
                    else
                        await _userManager.AddClaimsAsync(appUser, newClaims);

                    var mappedUser = _mapper.Map<OutputUser>(appUser);
                    result.Data = mappedUser;
                    result.Successful = true;                        
                }
                else
                    result.Messages.AddIdentityErrors(resultUpdate.Errors);
            }
            else
                result.Messages.AddValidationErrors(validationResult.Errors);
            return result;
        }
        public async Task<UserOperationResult> ChangeEmail(ApplicationUser appUser, EmailUpdate newEmail)
        {
            var result = new UserOperationResult();
            var validationResult = _emailUpdateValidator.Validate(newEmail);
            if (validationResult.IsValid)
            {
                var emailToken = await _userManager.GenerateChangeEmailTokenAsync(appUser, newEmail.NewEmail);
                var changeResult = await _userManager.ChangeEmailAsync(appUser, newEmail.NewEmail, emailToken);

                if (!changeResult.Succeeded)
                    result.Messages.AddIdentityErrors(changeResult.Errors);

                var mappedUser = _mapper.Map<OutputUser>(appUser);
                result.Data = mappedUser;
            }
            else
                result.Messages.AddValidationErrors(validationResult.Errors);

            return result;
        }
        public async Task<UserOperationResult> ChangePassword(ApplicationUser user, UserPasswordChange passwordChange)
        {
            var result = new UserOperationResult();
            var validationResult = _passwordChangeValidator.Validate(passwordChange);
            if (validationResult.IsValid)
            {
                var changeResult = await _userManager.ChangePasswordAsync(user, passwordChange.Password, passwordChange.NewPassword);
                result.Successful = changeResult.Succeeded;

                if (!changeResult.Succeeded)
                    result.Messages.AddIdentityErrors(changeResult.Errors);

                var mappedUser = _mapper.Map<OutputUser>(user);
                result.Data = mappedUser;
            }
            else
                result.Messages.AddValidationErrors(validationResult.Errors);

            return result;
        }
        public async Task<UserOperationResult> RecoverPassword(string email)
        {
            var user = await Get(email);
            if(user is null)
                return new UserOperationResult(false, new List<string> { "Usuário não encontrado" });
            
            await _passwordRecovery.Send(user);
            return new UserOperationResult(true, new List<string> { $"E-mail de recuperação encaminhado para {email}" });
        }
        public async Task<UserOperationResult> ResetPassword(UserPasswordReset passwordReset)
        {
            var result = new UserOperationResult();
            var validationResult = _passwordResetValidator.Validate(passwordReset);
            if (validationResult.IsValid)
            {
                var user = await Get(passwordReset.Email);
                var resetResult = await _userManager.ResetPasswordAsync(user, passwordReset.Token, passwordReset.Password);
                if(!resetResult.Succeeded)
                    result.Messages.AddIdentityErrors(resetResult.Errors);

                var mappedUser = _mapper.Map<OutputUser>(user);
                result.Data = mappedUser;
                result.Successful = resetResult.Succeeded;

                if(resetResult.Succeeded)
                    result.Messages.Add("Senha atualizada com sucesso.");
            }
            else
                result.Messages.AddValidationErrors(validationResult.Errors);

            return result;
        }
        public async Task<UserOperationResult> ConfirmUser(string email, string token)
        {
            var result = new UserOperationResult();
            var appUser = await Get(email);
            if(appUser is null)
            {
                result.Messages.Add("Usuário não encontrado, tente novamente.");
                return result;
            }

            var confirmationResult = await _userManager.ConfirmEmailAsync(appUser, token);
            if (!confirmationResult.Succeeded)
                result.Messages.AddIdentityErrors(confirmationResult.Errors);

            var mappedUser = _mapper.Map<OutputUser>(appUser);
            result.Data = mappedUser;
            result.Successful = confirmationResult.Succeeded;
            if (confirmationResult.Succeeded)
                result.Messages.Add("Cadastro confirmado com sucesso.");

            return result;
        }
        public async Task<bool> AcceptUserTerms(ApplicationUser user)
        {
            user.TermsAgreed = true;
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }
        public async Task<bool> IsUserPlanActive(ApplicationUser user)
        {
            var userPlan = _usersPlansRepository.Get().FirstOrDefault(up => up.UserId == user.Id);
            return userPlan is null ? 
                false : userPlan.IsActive && userPlan.Maturity >= DateTime.Now;
        }
        public void ActivateUserPlan(ApplicationUser user, bool isActive)
        {
            var userPlan = _usersPlansRepository.Get().FirstOrDefault(up => up.UserId == user.Id);
            if (userPlan is null)
                return;
            userPlan.IsActive = isActive;

            _usersPlansRepository.Update(userPlan);
            _usersPlansRepository.SaveChanges();
        }
        public async Task AddToPlan(ApplicationUser user, ServicePlan plan, bool isActive = true)
        {
            var userPlan = new ApplicationUserPlan
            {
                UserId = user.Id,
                RoleId = plan.Id,
                IsActive = isActive,
                Maturity = plan.Maturity.Equals(TimeSpan.MaxValue) ? DateTime.MaxValue : DateTime.Now + plan.Maturity
            };
            var result = _usersPlansRepository.Add(userPlan);
            _usersPlansRepository.SaveChanges();
        }
        public async Task<IEnumerable<Claim>> GetClaims(ApplicationUser user, bool newClaims = false, string planName = "")
        {
            var existingClaims = await _userManager.GetClaimsAsync(user);
            if(!newClaims && existingClaims is not null && existingClaims.Any())
                return existingClaims.ToList();

            var plans = await _userManager.GetRolesAsync(user);
            planName = string.IsNullOrEmpty(planName) ? plans.FirstOrDefault() : planName;

            return  new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, planName)
            };
        }
        public ApplicationUserPlan? GetUserPlan(ApplicationUser user) => _usersPlansRepository.Get().FirstOrDefault(up => up.UserId == user.Id);

        public void RenewUserPlan(ApplicationUser user)
        {
            var userPlan = GetUserPlan(user);
            if(userPlan is null) return;

            var plan = _roleManager.Roles.FirstOrDefault(q => q.Id == userPlan.RoleId);
            if(plan is null) return;

            userPlan.Maturity += plan.Maturity;

            _usersPlansRepository.Update(userPlan);
            _usersPlansRepository.SaveChanges();
        }
        private void UpdateApplicationUserFields(ApplicationUser applicationUser, UserSignUp userDto)
        {
            applicationUser.Email = userDto.Email;
            applicationUser.Name = userDto.Name;
            applicationUser.BirthDate = userDto.BirthDate;
            applicationUser.UserName = userDto.UserName;
        }
    }
}
