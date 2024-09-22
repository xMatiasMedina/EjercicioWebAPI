namespace EjercicioWebAPI.DTOs
{
    public class ReviewsDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int ClaseId { get; set; }
        public float Puntaje { get; set; }
    }
}
