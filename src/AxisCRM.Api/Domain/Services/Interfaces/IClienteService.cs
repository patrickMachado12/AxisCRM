using AxisCRM.Api.DTO.Cliente;

namespace AxisCRM.Api.Domain.Services.Interfaces
{
    public interface IClienteService : IService<ClienteRequestDTO, ClienteResponseDTO, int>
    {
        Task<ClienteResponseDTO> ObterPorCpfCnpj(string cpfCnpj);
    }
}