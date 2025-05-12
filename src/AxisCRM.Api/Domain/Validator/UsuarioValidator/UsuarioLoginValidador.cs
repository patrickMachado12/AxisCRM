using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using AxisCRM.Api.DTO.Usuario;

namespace AxisCRM.Api.Domain.Validator
{
    public class UsuarioLoginValidador : AbstractValidator<UsuarioLoginRequestDTO>
    {
        public UsuarioLoginValidador() {
            
            RuleFor(x => x.Email).NotEmpty().WithMessage("O campo email deve ser preenchido.")
                .NotNull().WithMessage("O email não pode ser nulo.")
                .EmailAddress().WithMessage("O campo email deve ser um email válido.");
                
            RuleFor(x => x.Senha)
                .NotNull().WithMessage("O campo senha não pode ser nulo.")
                .NotEmpty().WithMessage("O campo senha deve ser preenchido.");

            RuleFor(x => x.Senha)
                .MinimumLength(6).WithMessage("O campo senha deve ter no mínimo 6 caracteres.")
                .Must(senha => !senha.Contains(" "))
                    .WithMessage("O campo senha não pode conter espaços em branco.")
                .When(x => !string.IsNullOrEmpty(x.Senha));
        }    
    }
}