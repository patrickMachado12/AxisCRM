using FluentValidation.TestHelper;
using AxisCRM.Api.Domain.Validator;
using AxisCRM.Api.DTO.Usuario;

namespace AxisCRM.Test.Validators
{
    public class UsuarioLoginValidadorTests
    {
        private readonly UsuarioLoginValidador _validator;

        public UsuarioLoginValidadorTests()
        {
            _validator = new UsuarioLoginValidador();
        }

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("")]
        public void DeveOcorrerErro_Quando_EmailForNullOuVazio(string email)
        {
            var dto = new UsuarioLoginRequestDTO { Email = email, Senha = "senhaValida" };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Theory]
        [InlineData("EmailInvalido")]
        [InlineData("invalido@")]
        [InlineData("@invalido")]
        [InlineData("@invalido.com")]
        [InlineData("invalido.com")]
        public void DeveOcorrerErro_Quando_EmailForInvalido(string email)
        {
            var dto = new UsuarioLoginRequestDTO { Email = email, Senha = "senhaValida" };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void NaoDeveOcorrerErro_Quando_EmailForValido()
        {
            var dto = new UsuarioLoginRequestDTO { Email = "usuario@teste.com", Senha = "senhaValida" };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("")]
        public void DeveOcorrerErro_Quando_SenhaForNullOuVazia(string senha)
        {
            var dto = new UsuarioLoginRequestDTO { Email = "usuario@teste.com", Senha = senha };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Senha);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("12345")]
        [InlineData("abc12")]
        public void DeveOcorrerErro_Quando_SenhaForMenorQue6Caracteres(string senha)
        {
            var dto = new UsuarioLoginRequestDTO { Email = "usuario@teste.com", Senha = senha };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Senha);
        }

        [Theory]
        [InlineData("abc123 ")]
        [InlineData(" abc123")]
        [InlineData("abc 123")]
        public void DeveOcorrerErro_Quando_SenhaConterEspacos(string senha)
        {
            var dto = new UsuarioLoginRequestDTO { Email = "usuario@teste.com", Senha = senha };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Senha);
        }

        [Fact]
        public void NaoDeveOcorrerErro_Quando_SenhaForValida()
        {
            var dto = new UsuarioLoginRequestDTO { Email = "usuario@teste.com", Senha = "senhaValida" };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Senha);
        }
    }
}