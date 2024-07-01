using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Models.Users
{
    public class UserSignUp : UserBase
    {
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PasswordConfirm { get; set; } = string.Empty;
        public int PlanId { get; set; }
        public DateTime BirthDate { get; set; }
        public bool TermsAccepted { get; set; }
    }
}
