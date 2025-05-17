using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AxisCRM.Api.Domain.Enums;
using AxisCRM.Api.Domain.Extensoes;
using AxisCRM.Api.Domain.Helper;
using AxisCRM.Api.Domain.Models;
using AxisCRM.Api.Domain.Repository.Interfaces;
using AxisCRM.Api.Domain.Services.Exceptions;
using AxisCRM.Api.Domain.Services.Interfaces;
using AxisCRM.Api.DTO;
using AxisCRM.Api.DTO.Usuario;

namespace AxisCRM.Api.Domain.Services.Classes
{
    public class UsuarioService : IUsuarioService
    {
        private const int TAMANO_MAXIMO_PAGINA = 100;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;
        private readonly TokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
    
        public UsuarioService(IUsuarioRepository usuarioRepository,
            IMapper mapper,
            TokenService tokenService,
            IHttpContextAccessor httpContextAccessor)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UsuarioLoginResponseDTO> Autenticar(UsuarioLoginRequestDTO usuarioLoginRequest)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(usuarioLoginRequest.Email);

            var hashSenha = GerarHashSenha(usuarioLoginRequest.Senha);

            if (usuario == null || usuario.Senha != hashSenha)
                throw new AuthenticationException("Usuário e/ou senha inválido(s).");

            return new UsuarioLoginResponseDTO
            {
                Email = usuario.Email,
                Token = _tokenService.GerarToken(usuario)
            };
        }

        public async Task<UsuarioResponseDTO> Adicionar(UsuarioRequestDTO entidade)
        {
            var logado = await ObterUsuarioLogadoAsync();
            GarantirPermissaoAdmin(logado);
            
            if (await _usuarioRepository.ObterPorEmailAsync(entidade.Email) != null)
                throw new BadRequestException("Já existe um usuário cadastrado com este e-mail.");

            if (!entidade.Perfil.HasValue)
                entidade.Perfil = PerfilUsuario.Padrao;
                
            var entity = _mapper.Map<Usuario>(entidade);
            entity.Senha = GerarHashSenha(entidade.Senha);
            entity.DataCadastro = DateTime.Today;

            await _usuarioRepository.AdicionarAsync(entity);
            return _mapper.Map<UsuarioResponseDTO>(entity);
        }

        public async Task<UsuarioResponseDTO> Atualizar(int id, UsuarioRequestDTO entidade)
        {
            var existente = await _usuarioRepository.ObterPorIdAsync(id)
                            ?? throw new NotFoundException("Usuário não encontrado para atualização.");

            _mapper.Map(entidade, existente);

            if (!string.IsNullOrWhiteSpace(entidade.Senha))
                existente.Senha = GerarHashSenha(entidade.Senha);

            if (entidade.Perfil.HasValue)
                existente.Perfil = entidade.Perfil.Value;

            if (entidade.Perfil.HasValue)
            {
                var novoPerfil = entidade.Perfil.Value;
                if (!Enum.IsDefined(typeof(PerfilUsuario), novoPerfil))
                    throw new BadRequestException("Perfil de usuário inválido.");

                existente.Perfil = novoPerfil;
            }

            var outroComMesmoEmail = await _usuarioRepository.ObterPorEmailAsync(existente.Email);
            RegraNegocio.Validate(
                (outroComMesmoEmail == null || outroComMesmoEmail.Id == existente.Id,
                "Já existe outro usuário com este e-mail."),
                (Enum.IsDefined(typeof(PerfilUsuario), existente.Perfil),
                "Perfil de usuário inválido.")
            );

            await _usuarioRepository.AtualizarAsync(existente);

            return _mapper.Map<UsuarioResponseDTO>(existente);
        }

        public async Task<UsuarioResponseDTO> Excluir(int id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id)
                ?? throw new NotFoundException($"Usuário (ID {id}) não encontrado para exclusão.");

            if (usuario.Excluido)
                throw new BadRequestException("Usuário já está excluído.");

            var emailLogado = _httpContextAccessor.ObterEmailUsuarioLogado();

            if (string.IsNullOrWhiteSpace(emailLogado))
                throw new UnauthorizedAccessException("Não foi possível identificar o usuário logado.");

            var usuarioLogado = await _usuarioRepository.ObterPorEmailAsync(emailLogado)
                ?? throw new NotFoundException("Usuário logado não encontrado.");

            if (usuarioLogado.Id == id)
                throw new BadRequestException("Você não pode excluir a si mesmo.");

            if (usuarioLogado.Perfil != PerfilUsuario.Admin)
                throw new UnauthorizedAccessException("Apenas administradores podem excluir usuários.");

            usuario.Excluido = true;
            usuario.DataExclusao = DateTime.Now;   
            
            await _usuarioRepository.AtualizarAsync(usuario);

            return _mapper.Map<UsuarioResponseDTO>(usuario);
        }

        public async Task<UsuarioResponseDTO> ObterPorEmail(string email)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(email)
                ?? throw new NotFoundException("Usuario não encontrado.");

            return _mapper.Map<UsuarioResponseDTO>(usuario);
        }

        public async Task<UsuarioResponseDTO> ObterPorId(int id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id)
                ?? throw new NotFoundException("Usuario não encontrado.");
                
            return _mapper.Map<UsuarioResponseDTO>(usuario);
        }

        public async Task<PaginacaoResponseDTO<UsuarioResponseDTO>> ObterTodos(PaginacaoRequestDTO paginacao)
        {
            var tamanhoValido = Math.Min(paginacao.TamanhoPagina, TAMANO_MAXIMO_PAGINA);

            (IEnumerable<Usuario> usuarios, int totalItens) =
                await _usuarioRepository.ObterPaginadoAsync(
                    paginacao.Pagina,
                    paginacao.TamanhoPagina
                );

            var usuariosDTO = _mapper.Map<IEnumerable<UsuarioResponseDTO>>(usuarios);

            return new PaginacaoResponseDTO<UsuarioResponseDTO>
            {
                Itens = usuariosDTO,
                TotalItens = totalItens,
                PaginaAtual = paginacao.Pagina,
                TamanhoPagina = tamanhoValido,
                TotalPaginas = (int)Math.Ceiling((double)totalItens / tamanhoValido)
            };
        }

        private string GerarHashSenha(string senha)
        {
            string hashSenha;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytesSenha = Encoding.UTF8.GetBytes(senha);
                byte[] bytesHashSenha = sha256.ComputeHash(bytesSenha);
                hashSenha = BitConverter.ToString(bytesHashSenha).Replace("-", "").Replace("-", "").ToLower();
            }

            return hashSenha;
        }

        private async Task<Usuario> ObterUsuarioLogadoAsync()
        {
            var email = _httpContextAccessor.ObterEmailUsuarioLogado();
            return await _usuarioRepository
                .ObterPorEmailAsync(email)
                ?? throw new NotFoundException("Usuário logado não encontrado.");
        }

        private void GarantirPermissaoAdmin(Usuario usuario)
        {
            if (usuario.Perfil != PerfilUsuario.Admin)
                throw new UnauthorizedAccessException("Apenas administradores podem realizar esta operação.");
        }

    }
}