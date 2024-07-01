using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Models.Users
{
    public class UserPasswordChange : UserBase
    {
        public string NewPassword{ get; set; } = string.Empty;
        public string NewPasswordConfirm { get; set; } = string.Empty;
    }
}
