using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Domain.Models;

namespace AxisCRM.Api.Domain.Repository.Interfaces
{
    public interface IClienteRepository : IRepository<Cliente, int>
    {
        Task<Cliente> ObterPorCpfCnpjAsync(string cpfCnpj);

    }
}