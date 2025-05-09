using AutoMapper;
using AxisCRM.Api.Domain.Enums;
using AxisCRM.Api.Domain.Models;
using AxisCRM.Api.DTO.Usuario;

namespace AxisCRM.Api.AutoMapper
{
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            CreateMap<UsuarioRequestDTO, Usuario>()
                .ForMember(dest => dest.Id,           opt => opt.Ignore())
                .ForMember(dest => dest.DataCadastro, opt => opt.Ignore())
                .ForMember(dest => dest.Senha,        opt => opt.Ignore())
                .ForMember(dest => dest.Email,        opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Perfil,       opt =>
                {
                    opt.MapFrom(src => src.Perfil ?? PerfilUsuario.Padrao);
                });

            CreateMap<Usuario, UsuarioResponseDTO>();
        }
    }
}
