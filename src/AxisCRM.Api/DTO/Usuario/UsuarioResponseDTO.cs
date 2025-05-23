namespace AxisCRM.Api.DTO.Usuario
{
    public class UsuarioResponseDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int Perfil { get; set; }
        public DateTime DataCadastro { get; set; }
        public bool Excluido { get; set; }
        public DateTime? DataExclusao { get; set; }
    }
}