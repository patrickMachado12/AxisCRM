using FluentValidation.TestHelper;
using AxisCRM.Api.DTO.Cliente;
using AxisCRM.Api.Domain.Enums;
using AxisCRM.Api.Domain.Validator.ClienteValidator;

namespace AxisCRM.Test.Validators
{
    public class ClienteValidadorTests
    {
        private readonly ClienteValidador _clienteValidator;

        public ClienteValidadorTests()
        {
            _clienteValidator = new ClienteValidador();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void DeveError_NomeNullOuVazio(string nome)
        {
            var dto = new ClienteRequestDTO { Nome = nome };

            var result = _clienteValidator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Nome);
        }

        [Theory]
        [InlineData("A")]
        public void DeveError_NomeMenorQue2(string nome)
        {
            var dto = new ClienteRequestDTO { Nome = nome };

            var result = _clienteValidator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Nome);
        }

        [Fact]
        public void DeveError_NomeMaiorQue100()
        {
            var nome = new string('X', 101);
            var dto = new ClienteRequestDTO { Nome = nome };

            var result = _clienteValidator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Nome);
        }

        [Fact]
        public void NaoDeveError_NomeValido()
        {
            var dto = new ClienteRequestDTO { Nome = "Cliente Exemplo" };

            var result = _clienteValidator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Nome);
        }

        [Fact]
        public void DeveError_TipoPessoaNull()
        {
            var dto = new ClienteRequestDTO { TipoPessoa = null };

            var result = _clienteValidator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.TipoPessoa);
        }

        [Theory]
        [InlineData((TipoPessoa)0)]
        [InlineData((TipoPessoa)3)]
        public void DeveError_TipoPessoaInvalido(TipoPessoa tipo)
        {
            var dto = new ClienteRequestDTO { TipoPessoa = tipo };

            var result = _clienteValidator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.TipoPessoa);
        }

        [Theory]
        [InlineData(TipoPessoa.Fisica)]
        [InlineData(TipoPessoa.Juridica)]
        public void NaoDeveError_TipoPessoaValido(TipoPessoa tipo)
        {
            var dto = new ClienteRequestDTO { TipoPessoa = tipo };

            var result = _clienteValidator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.TipoPessoa);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void DeveError_CpfCnpjNullOuVazio(string documento)
        {
            var dto = new ClienteRequestDTO { TipoPessoa = TipoPessoa.Fisica, CpfCnpj = documento };

            var result = _clienteValidator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.CpfCnpj);
        }

        [Fact]
        public void NaoDeveError_CpfValido()
        {
            var dto = new ClienteRequestDTO
            {
                TipoPessoa = TipoPessoa.Fisica,
                CpfCnpj = "529.982.247-25"
            };

            var result = _clienteValidator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.CpfCnpj);
        }

        [Fact]
        public void DeveError_CnpjInvalido()
        {
            var dto = new ClienteRequestDTO
            {
                TipoPessoa = TipoPessoa.Juridica,
                CpfCnpj = "12.345.678/0001-99"
            };

            var result = _clienteValidator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.CpfCnpj);
        }

        [Fact]
        public void NaoDeveError_CnpjValido()
        {
            var dto = new ClienteRequestDTO
            {
                TipoPessoa = TipoPessoa.Juridica,
                CpfCnpj = "12.345.678/0001-95"
            };

            var result = _clienteValidator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.CpfCnpj);
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void DeveError_EmailNullOuVazio(string email)
        {
            var dto = new ClienteRequestDTO { Email = email };

            var result = _clienteValidator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Theory]
        [InlineData("invalido")]
        [InlineData("invalido@")]
        [InlineData("@invalido.com")]
        public void DeveError_EmailFormatoInvalido(string email)
        {
            var dto = new ClienteRequestDTO { Email = email };

            var result = _clienteValidator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void NaoDeveError_EmailValido()
        {
            var dto = new ClienteRequestDTO { Email = "cliente@teste.com" };

            var result = _clienteValidator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void DeveError_TelefoneNullOuVazio(string tel)
        {
            var dto = new ClienteRequestDTO { Telefone = tel };

            var result = _clienteValidator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Telefone);
        }

        [Fact]
        public void DeveError_TelefoneMaiorQue20()
        {
            var tel = new string('1', 21);
            var dto = new ClienteRequestDTO { Telefone = tel };

            var result = _clienteValidator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Telefone);
        }

        [Fact]
        public void NaoDeveError_TelefoneValido()
        {
            var dto = new ClienteRequestDTO { Telefone = "(11) 98765-4321" };

            var result = _clienteValidator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Telefone);
        }

        [Fact]
        public void DeveError_ObservacaoMaiorQue500()
        {
            var obs = new string('O', 501);
            var dto = new ClienteRequestDTO { Observacao = obs };

            var result = _clienteValidator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Observacao);
        }

        [Fact]
        public void NaoDeveError_ObservacaoNullOuVazia()
        {
            var dto1 = new ClienteRequestDTO { Observacao = null };

            var dto2 = new ClienteRequestDTO { Observacao = "" };

            var dto3 = new ClienteRequestDTO { Observacao = " " };

            _clienteValidator.TestValidate(dto1).ShouldNotHaveValidationErrorFor(x => x.Observacao);

            _clienteValidator.TestValidate(dto2).ShouldNotHaveValidationErrorFor(x => x.Observacao);
            
            _clienteValidator.TestValidate(dto3).ShouldNotHaveValidationErrorFor(x => x.Observacao);
        }
    }
}
