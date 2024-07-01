using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Contracts;
using Transdit.Core.Domain;
using Transdit.Repository.Repositories;

namespace Transdit.Services.Roles
{
    [ExcludeFromCodeCoverage]
    public class PlansService : IPlansService
    {
        private readonly RoleManager<ServicePlan> _roleManager;
        private readonly IRepository<ApplicationUserPlan> _usersPlansRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public PlansService(RoleManager<ServicePlan> roleManager, IRepository<ApplicationUserPlan> usersPlansRepository, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _usersPlansRepository = usersPlansRepository;
            _userManager = userManager;
        }

        public async Task<ServicePlan> Get(string identifier)
        {            
            var plan = int.TryParse(identifier, out var id) ? await _roleManager.FindByIdAsync(id.ToString()) : await _roleManager.FindByNameAsync(identifier);
            return plan;
        }

        public IEnumerable<ServicePlan> Get()
        {
            //remove o plano admin da lista
            var plans = _roleManager.Roles.Where(plan => plan.Id != 6);

            return plans.ToList();
        }

        public async Task<ServicePlan> Get(ApplicationUser user)
        {
            var userPlan = _usersPlansRepository.Get().FirstOrDefault(up => up.UserId == user.Id);

            if (userPlan is null)
                return default;

            var plan = await _roleManager.FindByIdAsync(userPlan.RoleId.ToString());

            return plan;
        }

        public async Task<ICollection<ApplicationUser>> GetUsersInPlan(string planId)
        {
            var result = await _userManager.GetUsersInRoleAsync(planId);

            return result;
        }
        public async Task<ICollection<ApplicationUser>> GetUsersInPlan(ServicePlan plan)
        {
            var result = await _userManager.GetUsersInRoleAsync(plan.NormalizedName);

            return result;
        }
    }
}
