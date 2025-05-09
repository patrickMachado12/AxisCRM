using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AxisCRM.Api.DTO
{
    public class PaginacaoResponseDTO<T>
    {
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalItens { get; set; }
        public int TamanhoPagina { get; set; }
        public IEnumerable<T> Itens { get; set; }
    }
}