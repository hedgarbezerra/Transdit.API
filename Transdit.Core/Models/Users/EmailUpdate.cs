using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Models.Users
{
    public class EmailUpdate
    {
        public string NewEmail { get; set; } = string.Empty;
        public string NewEmailConfirmation { get; set; } = string.Empty;

    }
}
