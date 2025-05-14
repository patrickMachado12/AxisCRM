using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AxisCRM.Api.Domain.Models
{
    public class Parecer : EntidadeBase
    {
        public string Descricao { get; set; }
        public string PessoaContato { get; set; }
        public DateTime DataUltimaAlteracao { get; set; } = DateTime.Now;
        public int IdAtendimento { get; set; }
        public Atendimento Atendimento { get; set; }
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }
    }
}