using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AxisCRM.Api.Domain.Models;
using AxisCRM.Api.Domain.Repository.Interfaces;
using AxisCRM.Api.Domain.Services.Exceptions;
using AxisCRM.Api.Domain.Services.Interfaces;
using AxisCRM.Api.DTO;
using AxisCRM.Api.DTO.Atendimento;

namespace AxisCRM.Api.Domain.Services.Classes
{
    public class AtendimentoService : IAtendimentoService
    {
        private const int TAMANO_MAXIMO_PAGINA = 100;
        private readonly IAtendimentoRepository _atendimentoRepository;
        private readonly IParecerService _parecerService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;

        public AtendimentoService(
            IAtendimentoRepository atendimentoRepository,
            IParecerService parecerService,
            IUsuarioRepository usuarioRepository,
            IClienteRepository clienteRepository,
            IMapper mapper)
        {
            _atendimentoRepository = atendimentoRepository;
            _parecerService = parecerService;
            _usuarioRepository = usuarioRepository;
            _clienteRepository = clienteRepository;
            _mapper = mapper;
        }

        public async Task<AtendimentoResponseDTO> Adicionar(AtendimentoRequestDTO entidade)
        {
            if (await _usuarioRepository.ObterPorIdAsync(entidade.IdUsuario) == null)
                throw new BadRequestException($"Usuário não encontrado com id {entidade.IdUsuario}");

            if (await _clienteRepository.ObterPorIdAsync(entidade.IdCliente) == null)
                throw new BadRequestException($"Usuário não encontrado com id {entidade.IdCliente}");

            var entity = _mapper.Map<Atendimento>(entidade);
            entity.DataCadastro = DateTime.Now;
            entity.DataUltimaAtualizacao = DateTime.Now;
            await _atendimentoRepository.AdicionarAsync(entity);

            var responseDto = _mapper.Map<AtendimentoResponseDTO>(entity);

            if (entidade.Parecer != null)
            {
                entidade.Parecer.IdAtendimento = entity.Id;
                var parecerCriado = await _parecerService.Adicionar(entidade.Parecer);
                responseDto.Pareceres.Add(parecerCriado);
            }

            return _mapper.Map<AtendimentoResponseDTO>(entity);
        }

        public async Task<AtendimentoResponseDTO> Atualizar(int id, AtendimentoRequestDTO entidade)
        {
            var existente = await _atendimentoRepository.ObterPorIdAsync(id)
                            ?? throw new NotFoundException("Atendimento não encontrado para atualização.");

            _mapper.Map(entidade, existente);
            existente.DataUltimaAtualizacao = DateTime.Now;

            await _atendimentoRepository.AtualizarAsync(existente);

            return _mapper.Map<AtendimentoResponseDTO>(existente);
        }

        public async Task<AtendimentoResponseDTO> Excluir(int id)
        {
            var atendimento = await _atendimentoRepository.ObterPorIdAsync(id)
                ?? throw new NotFoundException($"Atendimento (ID {id}) não encontrado para exclusão.");

            if (atendimento.StatusEncerrado == true)
                throw new BadRequestException("Atendimento já encontra-se encerrado.");

            atendimento.StatusEncerrado = true;
            atendimento.DataEncerramento = DateTime.Now;

            await _atendimentoRepository.AtualizarAsync(atendimento);

            return _mapper.Map<AtendimentoResponseDTO>(atendimento);
        }

        public async Task<AtendimentoResponseDTO> ObterPorId(int id)
        {
            var atendimento = await _atendimentoRepository.ObterPorIdAsync(id)
                ?? throw new NotFoundException($"Atendimento {id} não foi encontrado");

            return _mapper.Map<AtendimentoResponseDTO>(atendimento);
        }

        public async Task<PaginacaoResponseDTO<AtendimentoResponseDTO>> ObterTodos(PaginacaoRequestDTO paginacao)
        {
            var tamanhoValido = Math.Min(paginacao.TamanhoPagina, TAMANO_MAXIMO_PAGINA);

            (IEnumerable<Atendimento> atendimentos, int totalItens) =
                await _atendimentoRepository.ObterPaginadoAsync(
                    paginacao.Pagina,
                    paginacao.TamanhoPagina
                );

            var atendimentosDTO = _mapper.Map<IEnumerable<AtendimentoResponseDTO>>(atendimentos);

            return new PaginacaoResponseDTO<AtendimentoResponseDTO>
            {
                Itens = atendimentosDTO,
                TotalItens = totalItens,
                PaginaAtual = paginacao.Pagina,
                TamanhoPagina = tamanhoValido,
                TotalPaginas = (int)Math.Ceiling((double)totalItens / tamanhoValido)
            };
        }
    }
}