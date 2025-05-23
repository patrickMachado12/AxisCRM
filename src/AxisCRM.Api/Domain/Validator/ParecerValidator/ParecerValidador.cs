using AxisCRM.Api.DTO.Parecer;
using FluentValidation;

namespace AxisCRM.Api.Domain.Validator.ParecerValidator
{
    public class ParecerValidador : AbstractValidator<ParecerRequestDTO>
    {
        public ParecerValidador()
        {
            RuleFor(x => x.Descricao)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("A descricao do parecer não pode ser vazia.")
                .NotNull().WithMessage("A descricao do parecer não pode ser nulo.");

            RuleFor(x => x.PessoaContato)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("A pessoa de contato não pode ser vazia.")
                .NotNull().WithMessage("A pessoa de contato não pode ser nulo.");
        }
    }
}