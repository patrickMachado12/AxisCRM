namespace AxisCRM.Api.DTO
{
    public class PaginacaoRequestDTO
    {
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 10;        
    }
}