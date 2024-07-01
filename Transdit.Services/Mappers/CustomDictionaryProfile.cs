using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Domain;
using Transdit.Core.Models.Pagination;
using Transdit.Core.Models.Transcription;
using Transdit.Core.Models.Users;

namespace Transdit.Services.Mappers
{
    public class CustomDictionaryProfile : Profile
    {
        public CustomDictionaryProfile()
        {
            CreateMap<InCustomDictionary, CustomDictionary>();
            CreateMap<OutCustomDictionary, CustomDictionary>();
            CreateMap<CustomDictionary, OutCustomDictionary>();

            CreateMap<CustomDictionaryWord, OutCustomDictionaryWord>();
            CreateMap<OutCustomDictionaryWord, CustomDictionaryWord>();

            CreateMap<CustomDictionary, OutCustomDictionary>()
                .ForMember(m => m.Words, f =>
                    f.MapFrom(src => 
                        src.Words.Select(w => new OutCustomDictionaryWord(w.Id, w.Word))));

            CreateMap<PaginatedList<CustomDictionary>, PaginatedList<OutCustomDictionary>>()
               .ForMember(p => p.Data, f => f.MapFrom(src => src.Data));
        }
    }
}
