using AutoMapper;
using AxisCRM.Api.Domain.Models;
using AxisCRM.Api.DTO.Cliente;

namespace AxisCRM.Api.AutoMapper
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {
            CreateMap<ClienteRequestDTO, Cliente>();
            CreateMap<Cliente, ClienteResponseDTO>();
        }
    }
}