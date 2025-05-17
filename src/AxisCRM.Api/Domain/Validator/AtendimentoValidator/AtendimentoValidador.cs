using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Domain.Validator.ParecerValidator;
using AxisCRM.Api.DTO.Atendimento;
using FluentValidation;

namespace AxisCRM.Api.Domain.Validator.AtendimentoValidator
{
    public class AtendimentoValidador : AbstractValidator<AtendimentoRequestDTO>
    {
        public AtendimentoValidador()
        {
            RuleFor(x => x.Assunto)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O assunto não pode ser vazio.")
                .NotNull().WithMessage("O assunto não pode ser nulo.");

            RuleFor(x => x.Parecer)
                .NotNull().WithMessage("É obrigatório incluir um parecer.");

            RuleFor(x => x.Parecer).SetValidator(new ParecerValidador());
        }
    }
}