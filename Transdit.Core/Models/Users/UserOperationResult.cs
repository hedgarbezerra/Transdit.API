using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Domain;

namespace Transdit.Core.Models.Users
{
    public class UserOperationResult : DefaultControllerResponse<OutputUser>
    {
        public UserOperationResult() {}
        public UserOperationResult(bool result) : base(result) { }
        public UserOperationResult(bool result, ICollection<string> messages) : base(result, messages)
        {
        }
    }
}
