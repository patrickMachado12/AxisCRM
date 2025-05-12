using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AxisCRM.Api.Domain.Validator
{
    public class UsuarioCadastroValidador : UsuarioValidadorBase
    {
        public UsuarioCadastroValidador()
        {
            RuleFor(x => x.Senha)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                    .WithMessage("O campo senha não pode ser nulo.")
                .NotEmpty()
                    .WithMessage("O campo senha deve ser preenchido.");

            RuleFor(x => x.Senha)
                .Cascade(CascadeMode.Stop)
                .MinimumLength(6)
                    .WithMessage("O campo senha deve ter no mínimo 6 caracteres.")
                .Must(senha => !senha.Contains(" "))
                    .WithMessage("O campo senha não pode conter espaços em branco.")
                .When(x => !string.IsNullOrEmpty(x.Senha));
        }
    }
}