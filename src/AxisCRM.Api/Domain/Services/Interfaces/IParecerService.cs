using AxisCRM.Api.DTO.Parecer;

namespace AxisCRM.Api.Domain.Services.Interfaces
{
    public interface IParecerService
    {
        Task<ParecerResponseDTO> AdicionarParecer(ParecerRequestDTO entidade);
        Task<ParecerResponseDTO> AtualizarParecer(int idAtendimento, int idParecer, ParecerEdicaoRequestDTO entidade);
        Task<ParecerResponseDTO> ObterParecerPorId(int idAtendimento, int idParecer);
    }
}