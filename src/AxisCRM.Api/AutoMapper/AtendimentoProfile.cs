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
                        
            CreateMap<AtendimentoEdicaoRequestDTO, Atendimento>()
                .ForMember(dest => dest.Pareceres, opt => opt.Ignore())
                .ForMember(dest => dest.Assunto,
                       opt => opt.Condition(src => src.Assunto != null))
                .ForMember(dest => dest.IdCliente,
                        opt => opt.Condition(src => src.IdCliente != null));
        }

    }
}