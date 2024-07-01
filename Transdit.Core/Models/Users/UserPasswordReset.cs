using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Models.Users
{
    public class UserPasswordReset : UserBase
    {
        public string PasswordConfirm { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
