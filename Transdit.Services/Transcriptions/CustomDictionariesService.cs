using AutoMapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Contracts;
using Transdit.Core.Domain;
using Transdit.Core.Models.Pagination;
using Transdit.Core.Models.Transcription;
using Transdit.Repository.Repositories;

namespace Transdit.Services.Transcriptions
{
    public interface ICustomDictionariesService
    {
        void AddWord(string word, CustomDictionary dictionary);
        void AddWord(string word, int dictId);
        void DeleteDictionary(int id, ApplicationUser user);
        void DeleteDictionary(string name, ApplicationUser user);
        void DeleteWord(int wordId, CustomDictionary dictionary);
        IEnumerable<CustomDictionary> GetDictionaries(ApplicationUser user);
        PaginatedList<CustomDictionary> GetDictionaries(PaginationInput pagination, string route, ApplicationUser user);
        CustomDictionary GetDictionary(int id);
        CustomDictionary GetDictionary(string name, ApplicationUser user);
        void NewDictionary(CustomDictionary dict);
        void NewDictionary(string name, string description, ApplicationUser user);
        void NewDictionary(string name, string description, IEnumerable<string> values, ApplicationUser user);
        void UpdateDictionary(CustomDictionary dict);
        void UpdateDictionary(string name, string description, int id);
    }

    public class CustomDictionariesService : ICustomDictionariesService
    {
        private readonly IRepository<CustomDictionary> _DictionaryRepository;
        private readonly IUriService _uriService;
        public CustomDictionariesService(IRepository<CustomDictionary> dictionaryRepository,  IUriService uriService)
        {
            _DictionaryRepository = dictionaryRepository;
            _uriService = uriService;
        }

        public void NewDictionary(string name, string description, IEnumerable<string> values, ApplicationUser user)
        {
            var words = values.Select(v => new CustomDictionaryWord(v)).ToList();
            var dict = new CustomDictionary { Name = name, Description = description, Words = words, UserId = user.Id };

            _DictionaryRepository.Add(dict);
            _DictionaryRepository.SaveChanges();
        }
        public void NewDictionary(string name, string description, ApplicationUser user)
        {
            var dict = new CustomDictionary { Name = name, Description = description, UserId = user.Id };

            _DictionaryRepository.Add(dict);
            _DictionaryRepository.SaveChanges();
        }
        public void NewDictionary(CustomDictionary dict)
        {
            _DictionaryRepository.Add(dict);
            _DictionaryRepository.SaveChanges();
        }

        public void UpdateDictionary(CustomDictionary dict)
        {
            _DictionaryRepository.Update(dict);
            _DictionaryRepository.SaveChanges();
        }
        public void UpdateDictionary(string name, string description, int id)
        {
            var dict = _DictionaryRepository.Get(id);
            if (dict is null)
                return;
            dict.Name = name;
            dict.Description = description;

            _DictionaryRepository.Update(dict);
            _DictionaryRepository.SaveChanges();
        }

        public void DeleteDictionary(string name, ApplicationUser user)
        {
            var dict = _DictionaryRepository.Get(q => q.Name.Equals(name) && q.UserId == user.Id).FirstOrDefault();
            if (dict is null)
                return;

            _DictionaryRepository.Delete(dict.Id);
            _DictionaryRepository.SaveChanges();
        }
        public void DeleteDictionary(int id, ApplicationUser user)
        {
            var dict = _DictionaryRepository.Get(id);
            if (dict is null || dict.UserId != user.Id)
                return;

            _DictionaryRepository.Delete(dict.Id);
            _DictionaryRepository.SaveChanges();
        }

        public CustomDictionary GetDictionary(string name, ApplicationUser user) =>
            _DictionaryRepository.Get(q => q.Name.Equals(name) && q.UserId == user.Id).FirstOrDefault();
        public CustomDictionary GetDictionary(int id) => _DictionaryRepository.Get(id);
        public IEnumerable<CustomDictionary> GetDictionaries(ApplicationUser user) => _DictionaryRepository.Get(d => d.UserId == user.Id).ToList();
        public PaginatedList<CustomDictionary> GetDictionaries(PaginationInput pagination, string route, ApplicationUser user)
        {
            pagination.Index += 1;
            var dicts = _DictionaryRepository.Get(d => d.UserId == user.Id);
            if (!string.IsNullOrEmpty(pagination.SearchTerm))
            {
                var actualTerm = pagination.SearchTerm.Split(' ');
                for (int i = 0; i < actualTerm.Length; i++)
                {
                    var term = actualTerm[i];
                    dicts = dicts.Where(t => t.Name.Contains(term));
                }
            }
            var paginated = new PaginatedList<CustomDictionary>(dicts, _uriService, route, pagination.Index, pagination.Size);
            return paginated;
        }

        public void AddWord(string word, CustomDictionary dictionary)
        {
            var wordObj = new CustomDictionaryWord(word) { IdDictionary = dictionary.Id };
            dictionary.Words.Add(wordObj);
            _DictionaryRepository.SaveChanges();
        }
        public void AddWord(string word, int dictId)
        {
            var dictionary = _DictionaryRepository.Get(dictId);

            var wordObj = new CustomDictionaryWord(word) { IdDictionary = dictionary.Id };
            dictionary.Words.Add(wordObj);
            _DictionaryRepository.SaveChanges();
        }

        public void DeleteWord(int wordId, CustomDictionary dictionary)
        {
            var word = dictionary.Words.FirstOrDefault(x => x.Id == wordId);
            if (word is null)
                return;

            dictionary.Words.Remove(word);
            _DictionaryRepository.SaveChanges();
        }
    }
}
