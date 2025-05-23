using AxisCRM.Api.DTO;

namespace AxisCRM.Api.Domain.Services.Interfaces
{
    public interface IService<Request, Response, Id> where Request : class
    {
        Task<Response> ObterPorId(Id id);
        Task<Response> Adicionar(Request entidade);
        Task<Response> Atualizar(Id id, Request entidade);
        Task<Response> Excluir(Id id);
        Task<PaginacaoResponseDTO<Response>> ObterTodos(PaginacaoRequestDTO paginacao);
    }
}