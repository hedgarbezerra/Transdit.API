using AutoMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Domain;
using Transdit.Core.Models.Plans;

namespace Transdit.Services.Mappers
{
    [ExcludeFromCodeCoverage]
    public class PlansProfile : Profile
    {
        public PlansProfile()
        {
            CreateMap<ServicePlan, OutputPlan>()
                .ForMember(p => p.MonthlyLimitUsageMinutes, f => f.MapFrom(src => src.MonthlyLimitUsage.TotalMinutes))
                .ForMember(p => p.Name, f => f.MapFrom(src => src.Name))
                .ForMember(p => p.Price, f => f.MapFrom(src => src.Price))
                .ForMember(p => p.AllowTranscriptionSaving, f => f.MapFrom(src => src.AllowTranscriptionSaving))
                .ForMember(p => p.Description, f => f.MapFrom(src => src.Description));
        }
    }
}
