using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using AxisCRM.Api.Domain.Enums;
using AxisCRM.Api.Domain.Validator;
using AxisCRM.Api.DTO.Usuario;
using Xunit;

namespace AxisCRM.Test.Validators
{
public class UsuarioEdicaoValidadorTests
    {
        private readonly UsuarioEdicaoValidador _usuarioEdicaoValidator;

        public UsuarioEdicaoValidadorTests()
        {
            _usuarioEdicaoValidator = new UsuarioEdicaoValidador();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void DeveOcorrerErro_Quando_EmailForNullOuVazio(string email)
        {
            var dto = new UsuarioRequestDTO 
                { 
                    Email = email, 
                    Senha = "senhaValida", 
                    Perfil = PerfilUsuario.Padrao 
                };

            var result = _usuarioEdicaoValidator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Theory]
        [InlineData("EmailInvalido")]
        [InlineData("invalido@")]
        [InlineData("@invalido")]
        [InlineData("@invalido.com")]
        [InlineData("invalido.com")]
        public void DeveOcorrerErro_Quando_EmailInvalido(string email)
        {
            var dto = new UsuarioRequestDTO 
                { 
                    Email = email, 
                    Senha = "senhaValida", 
                    Perfil = PerfilUsuario.Padrao 
                };

            var result = _usuarioEdicaoValidator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void NaoDeveOcorrerErro_Quando_EmailForValido()
        {
            var dto = new UsuarioRequestDTO 
                { 
                    Email = "usuario@teste.com", 
                    Senha = "senhaValida", 
                    Perfil = PerfilUsuario.Padrao 
                };

            var result = _usuarioEdicaoValidator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Theory]
        [InlineData(null)]
        public void NaoDeveOcorrerErro_Quando_SenhaForNull(string senha)
        {
            var dto = new UsuarioRequestDTO 
                { 
                    Email = "usuario@teste.com", 
                    Senha = senha, 
                    Perfil = PerfilUsuario.Padrao 
                };

            var result = _usuarioEdicaoValidator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Senha);
        }


        [Theory]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("   ")]
        [InlineData("    ")]
        public void DeveOcorrerErro_Quando_SenhaInformadaInvalida(string senha)
        {
            var dto = new UsuarioRequestDTO 
                { 
                    Email = "usuario@teste.com", 
                    Senha = senha, 
                    Perfil = PerfilUsuario.Padrao 
                };

            var result = _usuarioEdicaoValidator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Senha);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("12345")]
        [InlineData("abc12")]
        public void DeveOcorrerErro_Quando_SenhaForMenorQue6Caracteres(string senha)
        {
            var dto = new UsuarioRequestDTO 
                { 
                    Email = "usuario@teste.com", 
                    Senha = senha, Perfil = 
                    PerfilUsuario.Padrao 
                };

            var result = _usuarioEdicaoValidator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Senha);
        }

        [Theory]
        [InlineData("abc123 ")]
        [InlineData(" abc123")]
        [InlineData("abc 123")]
        public void DeveOcorrerErro_Quando_SenhaConterEspacos(string senha)
        {
            var dto = new UsuarioRequestDTO 
                { 
                    Email = "usuario@teste.com", 
                    Senha = senha,
                    Perfil = PerfilUsuario.Padrao 
                };

            var result = _usuarioEdicaoValidator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Senha);
        }

        [Fact]
        public void NaoDeveOcorrerErro_Quando_SenhaForValida()
        {
            var dto = new UsuarioRequestDTO 
                { 
                    Email = "usuario@teste.com", 
                    Senha = "senhaValida", 
                    Perfil = PerfilUsuario.Padrao 
                };

            var result = _usuarioEdicaoValidator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Senha);
        }

        [Fact]
        public void DeveOcorrerErro_Quando_PerfilForNull()
        {
            var dto = new UsuarioRequestDTO 
                { 
                    Email = "usuario@teste.com", 
                    Senha = "senhaValida", 
                    Perfil = null 
                };

            var result = _usuarioEdicaoValidator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Perfil);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(4)]
        public void DeveOcorrerErro_Quando_PerfilForInvalido(int perfil)
        {
            var dto = new UsuarioRequestDTO 
                { 
                    Email = "usuario@teste.com", 
                    Senha = "senhaValida", 
                    Perfil = (PerfilUsuario)perfil 
                };

            var result = _usuarioEdicaoValidator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Perfil);
        }

        [Theory]
        [InlineData(PerfilUsuario.Admin)]
        [InlineData(PerfilUsuario.Padrao)]
        [InlineData(PerfilUsuario.Moderador)]
        public void NaoDeveOcorrerErro_Quando_PerfilForValido(PerfilUsuario perfil)
        {
            var dto = new UsuarioRequestDTO 
                { 
                    Email = "usuario@teste.com", 
                    Senha = "senhaValida", 
                    Perfil = perfil 
                };

            var result = _usuarioEdicaoValidator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Perfil);
        }
    }
}