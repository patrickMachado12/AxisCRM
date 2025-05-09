using Xunit;
using Moq;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using AxisCRM.Api.DTO.Usuario;
using AxisCRM.Api.Domain.Repository.Interfaces;
using AxisCRM.Api.Domain.Services.Classes;
using AxisCRM.Api.Domain.Models;
using AxisCRM.Api.Domain.Services.Exceptions;
using Microsoft.Extensions.Configuration;
using AxisCRM.Api.Domain.Enums;
using AxisCRM.Api.DTO;
using System.Security.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AxisCRM.Test.Services
{
    public class UsuarioServiceTests
    {
        private readonly UsuarioService _service;
        private readonly Mock<IUsuarioRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<TokenService> _tokenServiceMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

        public UsuarioServiceTests()
        {
            _repositoryMock = new Mock<IUsuarioRepository>();
            _mapperMock = new Mock<IMapper>();
            var inMemorySettings = new Dictionary<string, string>
            {
                { "KeySecret", "MinhaChaveSecreta32CharsLongTest!" },
                { "HorasValidadeToken", "1" }
            };
            IConfiguration config = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
            _tokenServiceMock = new Mock<TokenService>(config);
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _service = new UsuarioService(
                _repositoryMock.Object, 
                _mapperMock.Object, 
                _tokenServiceMock.Object,
                _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task Autenticar_ComUsuarioNull_RetornaThrowsAuthenticationException()
        {
            _repositoryMock.Setup(r => r.ObterPorEmailAsync(It.IsAny<string>())).ReturnsAsync((Usuario)null);
            await Assert.ThrowsAsync<AuthenticationException>(() => _service.Autenticar(new UsuarioLoginRequestDTO()));
        }

        [Fact]
        public async Task Autenticar_ComSenhaInvalida_RetornaThrowsAuthenticationException()
        {
            var user = new Usuario { Id = 1, Email = "usuario@email.com", Senha = "123abc" };

            _repositoryMock.Setup(r => r.ObterPorEmailAsync(user.Email)).ReturnsAsync(user);

            await Assert.ThrowsAsync<AuthenticationException>(() => 
                _service.Autenticar(new UsuarioLoginRequestDTO { Email = user.Email, Senha = "abc123" }));
        }

        [Fact]
        public async Task ObterPorId_ComUsuarioNull_RetornaThrowsNotFoundException()
        {
            _repositoryMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync((Usuario)null);
            await Assert.ThrowsAsync<NotFoundException>(() => _service.ObterPorId(1));
        }

        [Fact]
        public async Task ObterPorId_ComUsuarioValido_RetornaResponse()
        {   
            var user = new Usuario 
                { 
                    Id = 1, 
                    Email = "teste@email.com", 
                    Senha = "abc123", 
                    Perfil = PerfilUsuario.Padrao, 
                    DataCadastro = DateTime.Today 
                };

            _repositoryMock.Setup(r => r.ObterPorIdAsync(user.Id)).ReturnsAsync(user);

            var dto = new UsuarioResponseDTO 
                { 
                    Id = user.Id, 
                    Email = user.Email, 
                    Perfil = (int)user.Perfil, 
                    DataCadastro = user.DataCadastro 
                };

            _mapperMock.Setup(m => m.Map<UsuarioResponseDTO>(user)).Returns(dto);

            var result = await _service.ObterPorId(user.Id);

            Assert.Equal(dto.Id, result.Id);
            Assert.Equal(dto.Email, result.Email);
        }

        [Fact]
        public async Task ObterTodos_RetornaPaginacao()
        {
            var paginacao = new PaginacaoRequestDTO { Pagina = 1, TamanhoPagina = 10 };

            var users = new List<Usuario> { new Usuario 
                { 
                    Id = 1, 
                    Email = "teste@email.com", 
                    Senha = "123456", 
                    Perfil = PerfilUsuario.Padrao, 
                    DataCadastro = DateTime.Today 
                }};

            _repositoryMock.Setup(r => r.ObterPaginadoAsync(1, 10)).ReturnsAsync((users, 1));

            var dtos = new List<UsuarioResponseDTO> { new UsuarioResponseDTO 
                { 
                    Id = 1, 
                    Email = "teste@email.com", 
                    Perfil = (int)PerfilUsuario.Padrao, 
                    DataCadastro = DateTime.Today 
                }};

            _mapperMock.Setup(m => m.Map<IEnumerable<UsuarioResponseDTO>>(users)).Returns(dtos);

            var result = await _service.ObterTodos(paginacao);

            Assert.Single(result.Itens);
            Assert.Equal(1, result.TotalItens);
        }

        [Fact]
        public async Task Atualizar_ComUsuarioNull_RetornaThrowsNotFound()
        {
            var dto = new UsuarioRequestDTO 
                { 
                    Email = "u@u.com", 
                    Senha = "p", 
                    Perfil = PerfilUsuario.Padrao 
                };

            _repositoryMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync((Usuario)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.Atualizar(1, dto));
        }

        [Fact]
        public async Task Atualizar_ComDadosValidos_AtualizaEntidade()
        {
            var existing = new Usuario { Id = 1, Email = "anterior@e.com", Senha = "123456", Perfil = PerfilUsuario.Padrao };
            var dto      = new UsuarioRequestDTO { Email = "novo@e.com", Senha = "senha", Perfil = PerfilUsuario.Moderador };

            _repositoryMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(existing);
            _mapperMock.Setup(m => m.Map(dto, existing))
                    .Callback<UsuarioRequestDTO, Usuario>((d, e) =>
                    {
                        e.Email  = d.Email;
                        e.Senha  = d.Senha;
                        e.Perfil = d.Perfil.Value;
                    });
            _repositoryMock.Setup(r => r.ObterPorEmailAsync(It.IsAny<string>()))
                        .ReturnsAsync((Usuario)null);

            var claimsIdentity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Email, "admin@e.com")
            }, "TestAuth");
            var httpContext    = new DefaultHttpContext { User = new ClaimsPrincipal(claimsIdentity) };
            _httpContextAccessorMock
                .Setup(a => a.HttpContext)
                .Returns(httpContext);

            _mapperMock.Setup(m => m.Map<UsuarioResponseDTO>(existing))
                    .Returns(new UsuarioResponseDTO {
                        Id = 1,
                        Email = dto.Email,
                        Perfil = (int)dto.Perfil.Value,
                        DataCadastro = existing.DataCadastro
                    });

            var result = await _service.Atualizar(1, dto);

            Assert.Equal(dto.Email, result.Email);
        }

        [Fact]
        public async Task ExcluirAsync_JaExcluido_RetornaThrowsBadRequest()
        {
            var user = new Usuario { Id = 1, Excluido = true };

            _repositoryMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(user);

            await Assert.ThrowsAsync<BadRequestException>(() => _service.Excluir(1));
        }

        [Fact]
        public async Task ExcluirAsync_ComDadosValidos_DeveExcluirComSucesso()
        {
            var target = new Usuario
            {
                Id       = 1,
                Email    = "u@e.com",
                Perfil   = PerfilUsuario.Padrao,
                Excluido = false
            };
            var admin = new Usuario
            {
                Id       = 2,
                Email    = "admin@e.com",
                Perfil   = PerfilUsuario.Admin,
                Excluido = false
            };

            _repositoryMock.Setup(r => r.ObterPorIdAsync(target.Id)).ReturnsAsync(target);
            _repositoryMock.Setup(r => r.ObterPorEmailAsync(admin.Email)).ReturnsAsync(admin);

            var claimsIdentity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Email, admin.Email)
            }, "TestAuth");
            var httpContext    = new DefaultHttpContext { User = new ClaimsPrincipal(claimsIdentity) };
            _httpContextAccessorMock
                .Setup(a => a.HttpContext)
                .Returns(httpContext);

            _repositoryMock.Setup(r => r.AtualizarAsync(target))
                        .Returns(Task.FromResult(target));

            var resp = await _service.Excluir(target.Id);

            _repositoryMock.Verify(r => r.AtualizarAsync(
                It.Is<Usuario>(u => u.Id == target.Id && u.Excluido)), Times.Once);
        }
    }
}

