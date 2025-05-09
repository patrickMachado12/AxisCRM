using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AxisCRM.Api.Domain.Models
{
    public class EntidadeBase
    {
        [Key]
        public int Id { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Today;
    }
}