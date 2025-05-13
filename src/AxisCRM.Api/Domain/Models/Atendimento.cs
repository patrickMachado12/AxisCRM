using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AxisCRM.Api.Domain.Models
{
    public class Atendimento : EntidadeBase
    {
        public string Assunto { get; set; }
        public string Descricao { get; set; }
        public bool StatusEncerrado { get; set; } = false;
        public DateTime? DataEncerramento { get; set; }
        public DateTime DataUltimaAtualizacao { get; set; } = DateTime.Now;
        public int IdCliente { get; set; }
        public int IdUsuario { get; set; }
        public Cliente Cliente { get; set; }
        public Usuario Usuario { get; set; }
    }
}