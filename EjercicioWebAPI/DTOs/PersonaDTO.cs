namespace EjercicioWebAPI.DTOs
{
    public class PersonaDTO
    {
        public int DNI { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;

        public int UsuarioId { get; set; }
    }
}
