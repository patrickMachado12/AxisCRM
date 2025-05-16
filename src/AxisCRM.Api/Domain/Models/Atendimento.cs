using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Domain.Enums;

namespace AxisCRM.Api.Domain.Models
{
    public class Atendimento : EntidadeBase
    {
        public int IdCliente { get; set; }
        public int IdUsuario { get; set; }
        public string Assunto { get; set; }
        public string Historico { get; set; } = $"{DateTime.Now.ToShortDateString()} - Criado atendimento.{Environment.NewLine} ";
        public DateTime? DataEncerramento { get; set; }
        public DateTime? DataUltimaAtualizacao { get; set; }
        public StatusAtendimento Status { get; set; } = StatusAtendimento.Aberto;

        [ForeignKey(nameof(IdCliente))]
        public virtual Cliente Cliente { get; set; }
        
        [ForeignKey(nameof(IdUsuario))]
        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<Parecer> Pareceres { get; set; } = [];
    }
}