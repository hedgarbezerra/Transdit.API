using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Models
{
    public class DefaultControllerResponse<T> where T : new()
    {
        public DefaultControllerResponse(bool successful, T data, ICollection<string> messages)
        {
            Successful = successful;
            Data = data;
            Messages = messages;
        }
        public DefaultControllerResponse(bool successful, ICollection<string> messages)
        {
            Successful = successful;
            Messages = messages;
            Data = new T();
        }
        public DefaultControllerResponse(bool successful, T data)
        {
            Successful = successful;
            Data = data;
        }
        public DefaultControllerResponse(bool successful)
        {
            Successful = successful;
            Data = new T();
        }
        public DefaultControllerResponse()
        {
            Data = new T();
        }


        public bool Successful { get; set; }
        public T Data { get; set; }
        public ICollection<string> Messages { get; set; } = new List<string>();
    }
}
