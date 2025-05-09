using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using AxisCRM.Api.DTO.Usuario;

namespace AxisCRM.Api.Domain.Validator
{
    public class UsuarioEdicaoValidador : UsuarioValidadorBase
    {
        public UsuarioEdicaoValidador()
        {
            RuleFor(x => x.Senha)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("O campo senha deve ser preenchido.")
                .MinimumLength(6)
                    .WithMessage("O campo senha deve ter no mínimo 6 caracteres.")
                .Must(s => !s.Contains(" "))
                    .WithMessage("O campo senha não pode conter espaços em branco.")
                .When(x => x.Senha != null);
        }
    }
}