using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Models.Users;

namespace Transdit.Core.Contracts
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> Authenticate(Login login);
        bool ValidateToken(string token);
    }
}
