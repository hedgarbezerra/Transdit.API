using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Domain;

namespace Transdit.Core.Contracts
{
    public interface IPlansService
    {
        IEnumerable<ServicePlan> Get();
        Task<ServicePlan> Get(string identifier);
        Task<ServicePlan> Get(ApplicationUser user);
        Task<ICollection<ApplicationUser>> GetUsersInPlan(string planId);
        Task<ICollection<ApplicationUser>> GetUsersInPlan(ServicePlan plan);
    }
}
