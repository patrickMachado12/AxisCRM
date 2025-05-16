using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AxisCRM.Api.Domain.Enums;
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
                throw new BadRequestException($"Usuário com id {entidade.IdUsuario} não encontrado. Verifique!");

            if (await _clienteRepository.ObterPorIdAsync(entidade.IdCliente) == null)
                throw new BadRequestException($"Cliente com id {entidade.IdCliente} não encontrado. Verifique!");

            var entity = _mapper.Map<Atendimento>(entidade);
            entity.DataCadastro = DateTime.Now;
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

        public async Task<AtendimentoResponseDTO> AlterarStatus(int id, AtendimentoEdicaoStatusRequestDTO RequestDTO)
        {
            // 1) Buscar no repositório (ou DbContext) o atendimento existente
            var atendimento = await _atendimentoRepository.ObterPorIdAsync(id);
            if (atendimento == null)
                throw new NotFoundException($"Atendimento {id} não encontrado.");

             // 2) Negócios: não pode colocar como “Aberto” (ele já nasce nesse estado)
            if (RequestDTO.Status == StatusAtendimento.Aberto)
                throw new BadRequestException("Não é permitido alterar o status para 'Aberto'.");

            // 3) Negócios: não pode reabrir se já estiver reaberto, nem encerrar se já estiver encerrado
            if (atendimento.Status == RequestDTO.Status)
            {
                var verbo = RequestDTO.Status == StatusAtendimento.Reaberto ? "reabrir" : "encerrar";
                throw new BadRequestException($"Não é possível {verbo} um atendimento que já encontra-se {RequestDTO.Status}.");
            }

            // 2) Atualizar o status
            atendimento.Status = RequestDTO.Status;

            // 3) Incrementar o histórico
            var acao   = RequestDTO.Status == StatusAtendimento.Reaberto ? "Reaberto"  : "Encerrado";
            var motivo = !string.IsNullOrWhiteSpace(RequestDTO.Motivo)
                        ? $" - Motivo: {RequestDTO.Motivo}"
                        : string.Empty;
            atendimento.Historico +=
                $"{DateTime.Now:dd/MM/yyyy HH:mm} - {acao}{motivo}{Environment.NewLine} ";

            // (Opcional) Atualizar data da última modificação
            atendimento.DataUltimaAtualizacao = DateTime.Now;
            atendimento.DataEncerramento = RequestDTO.Status == StatusAtendimento.Encerrado
                                 ? DateTime.Now 
                                 : null;

            // 4) Persistir no banco
            await _atendimentoRepository.AtualizarAsync(atendimento);
        
            // 5) Mapear para DTO e retornar
            return _mapper.Map<AtendimentoResponseDTO>(atendimento);
        }

        public async Task<AtendimentoResponseDTO> Atualizar(int id, AtendimentoRequestDTO entidade)
        {
            var existente = await _atendimentoRepository.ObterPorIdAsync(id)
                            ?? throw new NotFoundException("Atendimento não encontrado para atualização.");
            
            if (existente.Status == StatusAtendimento.Encerrado)
                throw new BadRequestException("Não é permitido atualizar um atendimento que encontra-se encerrado.");

            _mapper.Map(entidade, existente);
            existente.DataUltimaAtualizacao = DateTime.Now;

            await _atendimentoRepository.AtualizarAsync(existente);

            return _mapper.Map<AtendimentoResponseDTO>(existente);
        }

        public async Task<AtendimentoResponseDTO> AtualizarAtendimento(int idAtendimento, AtendimentoEdicaoRequestDTO entidade)
        {
            var existente = await _atendimentoRepository.ObterPorIdAsync(idAtendimento)
                            ?? throw new NotFoundException("Atendimento não encontrado para atualização.");
            
            if (existente.Status == StatusAtendimento.Encerrado)
                throw new BadRequestException("Não é permitido atualizar um atendimento que encontra-se encerrado.");

            _mapper.Map(entidade, existente);
            existente.DataUltimaAtualizacao = DateTime.Now;

            await _atendimentoRepository.AtualizarAsync(existente);

            return _mapper.Map<AtendimentoResponseDTO>(existente);
        }

    public async Task<AtendimentoResponseDTO> Excluir(int id)
        {
            var atendimento = await _atendimentoRepository.ObterPorIdAsync(id)
                ?? throw new NotFoundException($"Atendimento (ID {id}) não encontrado para exclusão.");

            if (atendimento.Status == StatusAtendimento.Encerrado)
                throw new BadRequestException("Atendimento já encontra-se encerrado.");

            atendimento.Status = StatusAtendimento.Encerrado;
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