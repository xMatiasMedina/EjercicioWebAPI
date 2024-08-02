namespace EjercicioWebAPI.DTOs
{
    public class CrearPersonaDTO
    {
        public int DNI { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;

        public int UsuarioId { get; set; }
    }
}
