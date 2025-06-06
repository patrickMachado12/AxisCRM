using System.ComponentModel.DataAnnotations.Schema;

namespace AxisCRM.Api.Domain.Models
{
    public class Parecer : EntidadeBase
    {
        public int IdAtendimento { get; set; }
        public int IdUsuario { get; set; }
        public string Descricao { get; set; }
        public string PessoaContato { get; set; }
        public DateTime? DataUltimaAlteracao { get; set; }

        [ForeignKey(nameof(IdAtendimento))]
        public virtual Atendimento Atendimento { get; set; }
        
        [ForeignKey(nameof(IdUsuario))]
        public Usuario Usuario { get; set; }
    }
}