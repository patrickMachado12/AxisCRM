using FluentValidation;
using AxisCRM.Api.Domain.Enums;
using AxisCRM.Api.DTO.Usuario;

namespace AxisCRM.Api.Domain.Validator
{
    public abstract class UsuarioValidadorBase : AbstractValidator<UsuarioRequestDTO>
    {
        protected UsuarioValidadorBase()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O campo email deve ser preenchido.")
                .Must(email => !email.Any(char.IsWhiteSpace))
                    .WithMessage("O campo email não pode conter espaços em branco.")
                .EmailAddress().WithMessage("O campo email deve ser um email válido.");

            RuleFor(x => x.Perfil)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("O campo perfil deve ser informado.")
                .Must(p => Enum.IsDefined(typeof(PerfilUsuario), p.Value))
                    .WithMessage("Perfil de usuário inválido.");
        }
    }
}