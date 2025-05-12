using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AxisCRM.Api.Domain.Helper;
using AxisCRM.Api.Domain.Models;
using AxisCRM.Api.Domain.Repository.Interfaces;
using AxisCRM.Api.Domain.Services.Exceptions;
using AxisCRM.Api.Domain.Services.Interfaces;
using AxisCRM.Api.DTO;
using AxisCRM.Api.DTO.Cliente;

namespace AxisCRM.Api.Domain.Services.Classes
{
    public class ClienteService : IClienteService
    {
        private const int TAMANO_MAXIMO_PAGINA = 100;
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClienteService(IClienteRepository clienteRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _clienteRepository = clienteRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        
        public async Task<ClienteResponseDTO> Adicionar(ClienteRequestDTO entidade)
        {            
            if (await _clienteRepository.ObterPorCpfCnpjAsync(entidade.CpfCnpj) != null)
                throw new BadRequestException("Já existe um cliente cadastrado com este CPF/CNPJ.");
                
            var entity = _mapper.Map<Cliente>(entidade);
            entity.DataCadastro = DateTime.Today;

            await _clienteRepository.AdicionarAsync(entity);

            return _mapper.Map<ClienteResponseDTO>(entity);
        }

        public async Task<ClienteResponseDTO> Atualizar(int id, ClienteRequestDTO entidade)
        {
            var existente = await _clienteRepository.ObterPorIdAsync(id)
                            ?? throw new NotFoundException("Cliente não encontrado para atualização.");

            _mapper.Map(entidade, existente);

            var outroComMesmoCpfCnpj = await _clienteRepository.ObterPorCpfCnpjAsync(existente.CpfCnpj);

            if (await _clienteRepository.ObterPorCpfCnpjAsync(outroComMesmoCpfCnpj.CpfCnpj) != null)
                throw new BadRequestException("Já existe um cliente cadastrado com este CPF/CNPJ.");

            await _clienteRepository.AtualizarAsync(existente);

            return _mapper.Map<ClienteResponseDTO>(existente);
        }

        public async Task<ClienteResponseDTO> Excluir(int id)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(id)
                ?? throw new NotFoundException($"Cliente (ID {id}) não encontrado para exclusão.");

            if (cliente.Excluido)
                throw new BadRequestException("Cliente já está excluído.");

            cliente.Excluido = true;
            cliente.DataExclusao = DateTime.Now;   
            
            await _clienteRepository.AtualizarAsync(cliente);

            return _mapper.Map<ClienteResponseDTO>(cliente);
        }

        public async Task<ClienteResponseDTO> ObterPorCpfCnpj(string cpfCnpj)
        {
            var cliente = await _clienteRepository.ObterPorCpfCnpjAsync(cpfCnpj)
                ?? throw new NotFoundException("Cliente não encontrado.");

            return _mapper.Map<ClienteResponseDTO>(cliente);
        }

        public async Task<ClienteResponseDTO> ObterPorId(int id)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(id)
                ?? throw new NotFoundException("Cliente não encontrado.");
                
            return _mapper.Map<ClienteResponseDTO>(cliente);
        }

        public async Task<PaginacaoResponseDTO<ClienteResponseDTO>> ObterTodos(PaginacaoRequestDTO paginacao)
        {
            var tamanhoValido = Math.Min(paginacao.TamanhoPagina, TAMANO_MAXIMO_PAGINA);

            (IEnumerable<Cliente> clientes, int totalItens) =
                await _clienteRepository.ObterPaginadoAsync(
                    paginacao.Pagina,
                    paginacao.TamanhoPagina
                );

            var clientesDTO = _mapper.Map<IEnumerable<ClienteResponseDTO>>(clientes);

            return new PaginacaoResponseDTO<ClienteResponseDTO>
            {
                Itens        = clientesDTO,
                TotalItens   = totalItens,
                PaginaAtual  = paginacao.Pagina,
                TamanhoPagina= tamanhoValido,
                TotalPaginas = (int)Math.Ceiling((double)totalItens / tamanhoValido)
            };
        }
    }
}