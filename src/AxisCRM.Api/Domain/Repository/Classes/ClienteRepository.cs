using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Data;
using AxisCRM.Api.Domain.Models;
using AxisCRM.Api.Domain.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AxisCRM.Api.Domain.Repository.Classes
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ApplicationContext _contexto;

        public ClienteRepository(ApplicationContext context)
        {
            _contexto = context;
        }

        public async Task<Cliente> AdicionarAsync(Cliente entidade)
        {
            await _contexto.Cliente.AddAsync(entidade);
            await _contexto.SaveChangesAsync();

            return entidade;
        }

        public async Task<Cliente> AtualizarAsync(Cliente entidade)
        {
            Cliente? entidadeBanco = await _contexto.Cliente
                                                        .Where(u => u.Id == entidade.Id)
                                                        .FirstOrDefaultAsync();

            _contexto.Entry(entidadeBanco).CurrentValues.SetValues(entidade);
            _contexto.Update<Cliente>(entidadeBanco);

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

        public async Task<(IEnumerable<Cliente> Clientes, int TotalItens)> ObterPaginadoAsync(int pagina, int tamanhoPagina)
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
            return await _contexto.Cliente.Where(u => u.Id == id)
                                                    .FirstOrDefaultAsync();
        }

        public async Task<Cliente> ObterPorCpfCnpjAsync(string cpfCnpj)
        {
            return await _contexto.Cliente.AsNoTracking()
                .Where(u => u.CpfCnpj == cpfCnpj)
                .FirstOrDefaultAsync();
        }
    }
}