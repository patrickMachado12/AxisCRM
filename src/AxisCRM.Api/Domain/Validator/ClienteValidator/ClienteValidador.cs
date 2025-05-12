using System.Text.RegularExpressions;
using AxisCRM.Api.DTO.Cliente;
using AxisCRM.Api.Domain.Enums;
using FluentValidation;
using AxisCRM.Api.Domain.Helper;

namespace AxisCRM.Api.Domain.Validator.ClienteValidator
{
    public class ClienteValidador : AbstractValidator<ClienteRequestDTO>
    {
        public ClienteValidador()
        {
            RuleFor(x => x.Nome)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .NotNull().WithMessage("O nome não pode ser nulo.")
                .MinimumLength(2).WithMessage("O nome conter no mínimo 2 caracteres.")
                .MaximumLength(100).WithMessage("O nome não pode ter mais que 100 caracteres.");

            RuleFor(x => x.TipoPessoa)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("O tipo de pessoa é obrigatório.")
                .Must(tp => tp == TipoPessoa.Fisica || tp == TipoPessoa.Juridica)
                    .WithMessage("O tipo de pessoa deve ser ‘Fisica’ (1) ou ‘Juridica’ (2).");

            RuleFor(x => x.CpfCnpj)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Cpf/Cnpj é obrigatório.")
                .NotNull().WithMessage("O Cpf/Cnpj não pode ser nulo.")
                .Must((dto, cpfCnpj) => CpfCnpjHelper.IsValid(dto.TipoPessoa, cpfCnpj))
                    .WithMessage("Cpf/Cnpj inválido para o tipo de pessoa selecionado.");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O email é obrigatório.")
                .NotNull().WithMessage("O email não pode ser nulo.")
                .EmailAddress().WithMessage("O email deve ser um endereço válido.")
                .MaximumLength(100).WithMessage("O email não pode ter mais que 100 caracteres.");

            RuleFor(x => x.Telefone)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O telefone é obrigatório.")
                .NotNull().WithMessage("O telefone não pode ser nulo.")
                .MaximumLength(20).WithMessage("O telefone não pode ter mais que 20 caracteres.");

            RuleFor(x => x.Observacao)
                .MaximumLength(500).WithMessage("A observação não pode ter mais que 500 caracteres.");
        }
    }
}
