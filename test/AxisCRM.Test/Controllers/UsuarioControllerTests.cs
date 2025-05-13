using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using FluentValidation.Results;
using AxisCRM.Api.Controllers;
using AxisCRM.Api.Domain.Services.Interfaces;
using AxisCRM.Api.DTO.Usuario;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using AxisCRM.Api.DTO;
using AxisCRM.Api.Domain.Enums;
using AxisCRM.Api.Domain.Services.Exceptions;
using AxisCRM.Api.Domain.Validator;

namespace AxisCRM.Test.Controllers
{
    public class UsuarioControllerTests
    {
        private readonly UsuarioController _controller;
        private readonly Mock<IUsuarioService> _serviceMock;
        private readonly Mock<IValidator<UsuarioLoginRequestDTO>> _loginValidatorMock;
        private readonly UsuarioCadastroValidador _cadastroValidator;
        private readonly UsuarioEdicaoValidador _edicaoValidator;

        public UsuarioControllerTests()
        {
            _serviceMock = new Mock<IUsuarioService>();
            _cadastroValidator = new UsuarioCadastroValidador();
            _edicaoValidator = new UsuarioEdicaoValidador();
            _loginValidatorMock = new Mock<IValidator<UsuarioLoginRequestDTO>>();
            _controller = new UsuarioController
            (
                _serviceMock.Object,
                _cadastroValidator, 
                _edicaoValidator,
                _loginValidatorMock.Object
            );            
        }

        [Fact]
        public async Task Autenticacao_Invalida_RetornaBadRequest()
        {
            var dto = new UsuarioLoginRequestDTO { Email = "", Senha = "" };
            _loginValidatorMock.Setup(v => v.Validate(dto)).Returns(new ValidationResult(
                new List<ValidationFailure> { new ValidationFailure("Email", "Erro") }
            ));

            var result = await _controller.Autenticar(dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Autenticacao_ComDadosValidos_RetornaOK()
        {
            var dto = new UsuarioLoginRequestDTO { Email = "user@example.com", Senha = "validPassword" };

            _loginValidatorMock.Setup(v => v.Validate(dto)).Returns(new ValidationResult());

            var expectedResponse = new UsuarioLoginResponseDTO { Email = dto.Email, Token = "token" };

            _serviceMock.Setup(s => s.Autenticar(dto)).ReturnsAsync(expectedResponse);
            
            var result = await _controller.Autenticar(dto);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<UsuarioLoginResponseDTO>(ok.Value);

            Assert.Equal(expectedResponse.Email, value.Email);
            Assert.Equal(expectedResponse.Token, value.Token);
        }

        [Fact]
        public async Task Adicionar_ComDadosInvalidos_RetornaBadRequest()
        {
            var dto = new UsuarioRequestDTO { Email = "", Senha = "", Perfil = null };
            var failures = new List<ValidationFailure> { new ValidationFailure("Email", "Erro")};
            var fakeResult = new ValidationResult(failures);

            var result = await _controller.Adicionar(dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Adicionar_ComDadosValidos_RetornaOk()
        {
            var dto = new UsuarioRequestDTO 
            { 
                Email  = "user@example.com", 
                Senha  = "password", 
                Perfil = PerfilUsuario.Padrao 
            };

            var responseDto = new UsuarioResponseDTO 
            { 
                Id           = 1, 
                Email        = dto.Email, 
                Perfil       = (int)dto.Perfil.Value, 
                DataCadastro = DateTime.Today 
            };
            _serviceMock
            .Setup(s => s.Adicionar(dto))
            .ReturnsAsync(responseDto);

            var result = await _controller.Adicionar(dto);

            var ok    = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<UsuarioResponseDTO>(ok.Value);
            Assert.Equal(responseDto.Id,           value.Id);
            Assert.Equal(responseDto.Email,        value.Email);
            Assert.Equal(responseDto.Perfil,       value.Perfil);
            Assert.Equal(responseDto.DataCadastro, value.DataCadastro);
        }


        [Fact]
        public async Task ObterPorId_ComIdInvalido_RetornaNotFound()
        {
            _serviceMock.Setup(s => s.ObterPorId(It.IsAny<int>())).ThrowsAsync(new NotFoundException("Não encontrado"));

            var result = await _controller.ObterPorId(99);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task ObterPorId_ComIdValido_RetornaOk()
        {
            var dto = new UsuarioResponseDTO 
                { 
                    Id = 1, 
                    Email = "usuario@email.com", 
                    Perfil = (int)PerfilUsuario.Padrao, 
                    DataCadastro = DateTime.Today 
                };
            _serviceMock.Setup(s => s.ObterPorId(dto.Id)).ReturnsAsync(dto);

            var result = await _controller.ObterPorId(dto.Id);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<UsuarioResponseDTO>(ok.Value);

            Assert.Equal(dto.Id, value.Id);
        }

        [Fact]
        public async Task ObterTodos_RetornaUmaList()
        {
            var page = new PaginacaoRequestDTO { Pagina = 1, TamanhoPagina = 5 };
            var listDto = new PaginacaoResponseDTO<UsuarioResponseDTO>
            {
                Itens = new List<UsuarioResponseDTO> { new UsuarioResponseDTO 
                    { 
                        Id = 1, 
                        Email = "usuario@email.com", 
                        Perfil = (int)PerfilUsuario.Padrao, 
                        DataCadastro = DateTime.Today 
                    }},
                TotalItens = 1
            };
            _serviceMock.Setup(s => s.ObterTodos(page)).ReturnsAsync(listDto);

            var result = await _controller.ObterTodos(page);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<PaginacaoResponseDTO<UsuarioResponseDTO>>(ok.Value);

            Assert.Single(value.Itens);
        }

        [Fact]
        public async Task Atualizar_ComDadosInvalido_RetornaBadRequest()
        {
            var dto = new UsuarioRequestDTO 
            { 
                Email  = "", 
                Senha  = "", 
                Perfil = null 
            };

            var result = await _controller.Atualizar(1, dto);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }


        [Fact]
        public async Task Atualizar_ComIdInexistente_RetornaNotFound()
        {
            var dto = new UsuarioRequestDTO 
            { 
                Email  = "usuario@email.com", 
                Senha  = "abc123", 
                Perfil = PerfilUsuario.Padrao 
            };

            _serviceMock
                .Setup(s => s.Atualizar(1, dto))
                .ThrowsAsync(new NotFoundException("Não existe"));

            var result = await _controller.Atualizar(1, dto);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }


        [Fact]
        public async Task Atualizar_ComIdValido_RetornaOk()
        {
            var dto = new UsuarioRequestDTO 
            { 
                Email  = "usuario@email.com", 
                Senha  = "abc123", 
                Perfil = PerfilUsuario.Padrao 
            };
            var resp = new UsuarioResponseDTO 
            { 
                Id  = 1, 
                Email = dto.Email, 
                Perfil = (int)dto.Perfil.Value, 
                DataCadastro = DateTime.Today 
            }; 

            _serviceMock
                .Setup(s => s.Atualizar(1, dto))
                .ReturnsAsync(resp);

            var result = await _controller.Atualizar(1, dto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<UsuarioResponseDTO>(ok.Value);
            Assert.Equal(resp.Id, value.Id);
            Assert.Equal(resp.Email, value.Email);
            Assert.Equal(resp.Perfil, value.Perfil);
            Assert.Equal(resp.DataCadastro, value.DataCadastro);
        }


        [Fact]
        public async Task Excluir_ComIdInexistente_RetornaNotFound()
        {
            _serviceMock.Setup(s => s.Excluir(It.IsAny<int>())).ThrowsAsync(new NotFoundException("Não existe"));

            var result = await _controller.Excluir(99);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Excluir_ComIdValido_RetornaNoContent()
        {
            _serviceMock.Setup(s => s.Excluir(1)).ReturnsAsync(new UsuarioResponseDTO 
                { 
                    Id = 1, 
                    Email = "teste@email.com", 
                    Perfil = (int)PerfilUsuario.Padrao, 
                    DataCadastro = DateTime.Today 
                });

            var result = await _controller.Excluir(1);

            Assert.IsType<OkResult>(result.Result);
        }
    }
}
