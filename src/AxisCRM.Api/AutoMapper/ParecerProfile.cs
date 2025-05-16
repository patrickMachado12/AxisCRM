using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AxisCRM.Api.Domain.Models;
using AxisCRM.Api.DTO.Parecer;

namespace AxisCRM.Api.AutoMapper
{
    public class ParecerProfile : Profile
    {
        public ParecerProfile()
        {
            CreateMap<ParecerRequestDTO, Parecer>();
            CreateMap<Parecer, ParecerResponseDTO>();
            CreateMap<ParecerEdicaoRequestDTO, Parecer>()
                .ForMember(dest => dest.IdAtendimento, opt => opt.Ignore())
                .ForMember(dest => dest.IdUsuario,    opt => opt.Ignore()); 
        }
    }
}