using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transdit.Core.Domain
{
    public class CustomDictionary
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public virtual ICollection<CustomDictionaryWord> Words { get; set; }
        public virtual ApplicationUser User { get; set; }     
    }
    
    public class CustomDictionaryWord
    {
        public CustomDictionaryWord() { }
        public CustomDictionaryWord(string word)
        {
            Word = word;
        }
        public int Id { get; set; }
        public int IdDictionary { get; set; }
        public string Word { get; set; } = string.Empty;
    }
}
