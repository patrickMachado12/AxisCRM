using AxisCRM.Api.Domain.Models;

namespace AxisCRM.Api.Domain.Repository.Interfaces
{
    public interface IParecerRepository : IRepository<Parecer, int>
    {
        //TODO: Verificar se seria melhor criar um repository especifico para o Parecer.
    }
}