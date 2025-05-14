using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AxisCRM.Api.DTO.Parecer
{
    public class ParecerResponseDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public DateTime DataCadastro { get; set; }
        public string PessoaContato { get; set; }
        public int IdAtendimento { get; set; }
        public int IdUsuario { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
    }
}