using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Domain.Enums;

namespace AxisCRM.Api.Domain.Models
{
    public class Cliente : EntidadeBase
    {
        public string Nome { get; set; }
        public string CpfCnpj { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Observacao { get; set; }
        public bool Excluido { get; set; } = false;
        public TipoPessoa TipoPessoa { get; set; }
        public DateTime? DataExclusao { get; set; }
    }
}