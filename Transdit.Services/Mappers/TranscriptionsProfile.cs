using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Domain;
using Transdit.Core.Models.Pagination;
using Transdit.Core.Models.Transcription;
using Transdit.Core.Models.Transcriptions;

namespace Transdit.Services.Mappers
{
    public class TranscriptionsPlan : Profile
    {
        public TranscriptionsPlan()
        {
            CreateMap<Transcription, OutTranscription>()
                .ForMember(p => p.Id, f => f.MapFrom(src => src.Id))
                .ForMember(p => p.Name, f => f.MapFrom(src => src.Name))
                .ForMember(p => p.InputedFileName, f => f.MapFrom(src => src.InputedFileName))
                .ForMember(p => p.StorageFileName, f => f.MapFrom(src => src.StorageFileName))
                .ForMember(p => p.Date, f => f.MapFrom(src => src.Date))
                .ForMember(p => p.Result, f => f.MapFrom(src => src.Result))
                .ForMember(p => p.LengthInSeconds, f => f.MapFrom(src => src.Usage.TotalSeconds));

            CreateMap<InputTranscription, Transcription>()
                .ForMember(p => p.Name, f => f.MapFrom(src => string.IsNullOrEmpty(src.Name) ? Path.GetFileNameWithoutExtension(src.FileName) : src.Name))
                .ForMember(p => p.InputedFileName, f => f.MapFrom(src => src.FileName))
                .ForMember(p => p.StorageFileName, f => f.MapFrom(src => src.StorageFileName))
                .ForMember(p => p.Language, f => f.MapFrom(src => src.Language))
                .ForMember(p => p.Usage, f => f.MapFrom(src => TimeSpan.FromSeconds(src.LengthInSeconds)));

            CreateMap<PaginatedList<Transcription>, PaginatedList<OutTranscription>>();
        }
    }
}