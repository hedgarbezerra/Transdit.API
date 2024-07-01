using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Domain;
using Transdit.Core.Models.Pagination;
using Transdit.Core.Models.Users;

namespace Transdit.Core.Contracts
{
    public interface IUsersService
    {
        Task<UserOperationResult> ChangeEmail(ApplicationUser appUser, EmailUpdate newEmail);
        Task<UserOperationResult> ChangePassword(ApplicationUser user, UserPasswordChange passwordChange);
        Task<UserOperationResult> ConfirmUser(string email, string token);
        Task<UserOperationResult> Create(UserSignUp user);
        Task<PaginatedList<ApplicationUser>> Get(PaginationInput pagination, string route);
        Task<ApplicationUser> Get(string identifier);
        Task<ApplicationUser> Get(int id);
        Task<UserOperationResult> Update(UserSignUp user, string currentUser);
        Task<bool> IsUserPlanActive(ApplicationUser user);
        Task AddToPlan(ApplicationUser user, ServicePlan plan, bool isActive = true);
        void ActivateUserPlan(ApplicationUser user, bool isActive);
        Task<UserOperationResult> RecoverPassword(string email);
        Task<UserOperationResult> ResetPassword(UserPasswordReset passwordReset);
        Task<bool> AcceptUserTerms(ApplicationUser user);
        ApplicationUserPlan? GetUserPlan(ApplicationUser user);
        Task<IEnumerable<Claim>> GetClaims(ApplicationUser user, bool newClaims = false, string planName = "");
        void RenewUserPlan(ApplicationUser user);
    }
}
