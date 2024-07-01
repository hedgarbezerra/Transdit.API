using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Contracts
{
    public interface ICryptography
    {
        string Decrypt(string value);
        string Encrypt(string value);
    }
}
