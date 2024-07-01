using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Models.Notification;

namespace Transdit.Core.Contracts
{
    public interface IEmailFromFileGenerator
    {
        public string Generate(string fileName);
    }

}
