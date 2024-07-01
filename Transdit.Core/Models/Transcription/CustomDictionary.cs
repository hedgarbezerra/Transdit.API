using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Domain;

namespace Transdit.Core.Models.Transcription
{
    public class OutCustomDictionary
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public virtual ICollection<OutCustomDictionaryWord> Words { get; set; }
    }
    public class OutCustomDictionaryWord
    {
        public int Id { get; set; }
        public string Word { get; set; } = string.Empty;
        public OutCustomDictionaryWord()
        {            
        }
        public OutCustomDictionaryWord(string word)
        {
            Word = word;
        }

        public OutCustomDictionaryWord(int id, string word)
        {
            Id = id;
            Word = word;
        }
    }


    public class InCustomDictionary
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
