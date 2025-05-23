using System.ComponentModel.DataAnnotations;

namespace AxisCRM.Api.Domain.Models
{
    public class EntidadeBase
    {
        [Key]
        public int Id { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Today;
    }
}