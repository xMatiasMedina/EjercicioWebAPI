namespace EjercicioWebAPI.Entidades
{
    public class Inscripcion
    {
        public int Id { get; set; }
        public DateTime DiaHoraInscripcion { get; set; }
        public int ClaseId { get; set; }
        public int UsuarioId { get; set; }
    }
}
