using AxisCRM.Api.Domain.Enums;
using AxisCRM.Api.DTO.Atendimento;

namespace AxisCRM.Api.Domain.Services.Interfaces
{
    public interface IAtendimentoService
    {
        Task<AtendimentoResponseDTO> ObterPorId(int id);
        Task<AtendimentoResponseDTO> Adicionar(AtendimentoRequestDTO entidade);
        Task<AtendimentoResponseDTO> AlterarStatus(int id, AtendimentoEdicaoStatusRequestDTO RequestDTO);
        Task<AtendimentoResponseDTO> AtualizarAtendimento(int idAtendimento, AtendimentoEdicaoRequestDTO entidade);
        Task<IEnumerable<AtendimentoResponseDTO>> ObterAtendimentosFiltrados(
            int? idUsuario,
            int? idCliente,
            StatusAtendimento status,
            DateTime? dataInicial,
            DateTime? dataFinal);
    }
}