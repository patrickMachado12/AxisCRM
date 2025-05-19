using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Data;
using AxisCRM.Api.Domain.Models;
using AxisCRM.Api.Domain.Repository.Interfaces;
using AxisCRM.Api.Domain.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AxisCRM.Api.Domain.Repository.Classes
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ApplicationContext _contexto;

        public ClienteRepository(ApplicationContext context)
        {
            _contexto = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Cliente> AdicionarAsync(Cliente entidade)
        {
            await _contexto.Cliente.AddAsync(entidade);
            await _contexto.SaveChangesAsync();

            return entidade;
        }

        public async Task<Cliente> AtualizarAsync(Cliente entidade)
        {
            var entidadeBanco = await _contexto.Cliente
                .FirstOrDefaultAsync(u => u.Id == entidade.Id);

            if (entidadeBanco is null)
                throw new NotFoundException($"Cliente com id {entidade.Id} não foi encontrado.");

            _contexto.Entry(entidadeBanco).CurrentValues.SetValues(entidade);
            await _contexto.SaveChangesAsync();

            return entidadeBanco;
        }

        public Task ExcluirAsync(Cliente entidade)
        {
            _contexto.Update<Cliente>(entidade);
            return _contexto.SaveChangesAsync();
        }

        public async Task<IEnumerable<Cliente>> ObterAsync()
        {
            return await _contexto.Cliente.AsNoTracking()
                                                    .OrderBy(u => u.Id)
                                                    .ToListAsync();
        }

        public async Task<(IEnumerable<Cliente> entidades, int TotalItens)> ObterPaginadoAsync(int pagina, int tamanhoPagina)
        {
            var query = _contexto.Cliente.AsQueryable();
            var totalItens = await query.CountAsync();

            query = query.OrderBy(u => u.Id);

            var clientes = await query
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return (clientes, totalItens);
        }

        public async Task<Cliente> ObterPorIdAsync(int id)
        {
            var cliente = await _contexto.Cliente.Where(u => u.Id == id)
                                                .FirstOrDefaultAsync();
            if (cliente is null)
                throw new NotFoundException($"Cliente com id {id} não foi encontrado.");

            return cliente;                                         
        }

        public async Task<Cliente> ObterPorCpfCnpjAsync(string cpfCnpj)
        {
            return await _contexto.Cliente
                 .AsNoTracking()
                 .FirstOrDefaultAsync(u => u.CpfCnpj == cpfCnpj);
        }
    }
}