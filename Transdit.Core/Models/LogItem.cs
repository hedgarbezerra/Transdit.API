using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Transdit.Core.Models
{
    public enum LogLevel
    {
        //
        // Resumo:
        //     Anything and everything you might want to know about a running block of code.
        Verbose = 0,
        //
        // Resumo:
        //     Internal system events that aren't necessarily observable from the outside.
        Debug = 1,
        //
        // Resumo:
        //     The lifeblood of operational intelligence - things happen.
        Information = 2,
        //
        // Resumo:
        //     Service is degraded or endangered.
        Warning = 3,
        //
        // Resumo:
        //     Functionality is unavailable, invariants are broken or data is lost.
        Error = 4,
        //
        // Resumo:
        //     If you have a pager, it goes off when one of these occurs.
        Fatal = 5
    }
    public class LogItem
    {
        public int Id { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; } = string.Empty;
        public string MessageTemplate { get; set; } = string.Empty;
        public string Exception { get; set; } = string.Empty;
        public string Properties { get; set; } = string.Empty;
        [JsonIgnore]
        public XElement XmlContent
        {
            get { return XElement.Parse(Properties); }
            set { Properties = value.ToString(); }
        }
        public DateTime CreatedTime { get; set; }
        public LogItem()
        {
        }

        public LogItem(string message, string exception, LogLevel logLevel)
        {
            Message = message;
            Exception = exception;
            LogLevel = logLevel;
        }
        public LogItem(string message, string messageTemplate, string exception, string properties, LogLevel logLevel)
        {
            Message = message;
            MessageTemplate = messageTemplate;
            Exception = exception;
            Properties = properties;
            LogLevel = logLevel;
        }
    }
}
