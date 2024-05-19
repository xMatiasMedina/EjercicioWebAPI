namespace EjercicioWebAPI.Entidades
{
    public class Review
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int ClaseId { get; set; }
        public float Puntaje { get; set; }
    }
}
