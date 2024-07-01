using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Domain
{
    public class ApplicationUser: IdentityUser<int>
    {
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; } = DateTime.MinValue;
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public bool TermsAgreed { get; set; }
        public virtual ICollection<Transcription> Transcriptions { get; set; }
        public virtual ICollection<CustomDictionary> CustomDictionaries { get; set; }
    }
}
