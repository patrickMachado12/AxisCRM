using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AxisCRM.Api.DTO
{
    public class PaginacaoRequestDTO
    {
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 10;        
    }
}