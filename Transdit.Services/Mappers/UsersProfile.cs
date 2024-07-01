using AutoMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Domain;
using Transdit.Core.Models.Pagination;
using Transdit.Core.Models.Users;

namespace Transdit.Services.Mappers
{
    [ExcludeFromCodeCoverage]
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<ApplicationUser, OutputUser>()
                .ForMember(p => p.Name, f => f.MapFrom(src => src.Name))
                .ForMember(p => p.Username, f => f.MapFrom(src => src.NormalizedUserName))
                .ForMember(p => p.Email, f => f.MapFrom(src => src.NormalizedEmail))
                .ForMember(p => p.BirthDate, f => f.MapFrom(src => src.BirthDate))
                .ForMember(p => p.DateAdded, f => f.MapFrom(src => src.DateAdded))
                .ForMember(p => p.IsConfirmed, f => f.MapFrom(src => src.EmailConfirmed))
                .ForMember(p => p.TermsAgreed, f => f.MapFrom(src => src.TermsAgreed));


            CreateMap<PaginatedList<ApplicationUser>, PaginatedList<OutputUser>>()
               .ForMember(p => p.Data, f => f.MapFrom(src => src.Data));

        }
    }
}
