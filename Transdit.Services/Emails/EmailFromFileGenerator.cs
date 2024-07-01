using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Contracts;
using Transdit.Core.Models.Notification;

namespace Transdit.Services.Emails
{
    [ExcludeFromCodeCoverage]
    public class EmailFromFileGenerator : IEmailFromFileGenerator
    {
        private readonly string _rootFolder;
        public EmailFromFileGenerator( string rootFolder)
        {
            _rootFolder = rootFolder;
        }

        public string Generate(string fileName) => GetFile(fileName);

        private string GetFile(string fileName)
        {
            if (!File.Exists(_rootFolder + fileName))
            {
                return string.Empty;
            }
            return File.ReadAllText(_rootFolder + fileName);
        }
    }
}
