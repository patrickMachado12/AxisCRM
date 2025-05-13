using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AxisCRM.Api.DTO.Cliente
{
    public class ClienteResponseDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int TipoPessoa { get; set; }
        public string CpfCnpj { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Observacao { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Excluido { get; set; } = false;
        public DateTime? DataExclusao { get; set; }
    }
}