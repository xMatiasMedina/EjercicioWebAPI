namespace EjercicioWebAPI.DTOs
{
    public class CrearInscripcionesDTO
    {
        public DateTime DiaHoraInscripcion { get; set; }
        public int ClaseId { get; set; }
        public int UsuarioId { get; set; }
    }
}
