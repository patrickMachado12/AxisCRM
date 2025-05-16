using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.DTO.Atendimento;
using FluentValidation;

namespace AxisCRM.Api.Domain.Validator.AtendimentoValidator
{
    public class AtendimentoEdicaoValidador : AbstractValidator<AtendimentoRequestDTO>
    {
        public AtendimentoEdicaoValidador()
        {
            RuleFor(x => x.Assunto)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O assunto não pode ser vazio.")
                .NotNull().WithMessage("O assunto não pode ser nulo.");
        }
    }
}