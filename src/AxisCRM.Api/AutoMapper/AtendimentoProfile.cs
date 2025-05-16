using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AxisCRM.Api.Domain.Models;
using AxisCRM.Api.DTO.Atendimento;

namespace AxisCRM.Api.AutoMapper
{
    public class AtendimentoProfile : Profile
    {
        public AtendimentoProfile()
        {
            CreateMap<AtendimentoRequestDTO, Atendimento>();
            CreateMap<Atendimento, AtendimentoResponseDTO>()
                .ForMember(dest => dest.Pareceres,
                        opt => opt.MapFrom(src => src.Pareceres));
            CreateMap<AtendimentoEdicaoRequestDTO, Parecer>()
                .ForMember(dest => dest.IdAtendimento, opt => opt.Ignore());
        }

    }
}