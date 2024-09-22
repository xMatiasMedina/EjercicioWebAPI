namespace EjercicioWebAPI.DTOs
{
    public class ClaseDTO
    {
        public int Id { get; set; }
        public string Actividad { get; set; } = string.Empty;
        public int PreparadorFisicoId { get; set; }
        public DateTime DiaHorario { get; set; }
        public string Ubicacion { get; set; } = string.Empty;
        public float PuntajePromedio { get; set; }
        public float Precio { get; set; }
        public int CupoMax { get; set; }
        public string PreparadorFisicoNombre { get; set; } = string.Empty; // Example for related data
        public List<int> InscripcionesIds { get; set; } = new List<int>(); // IDs of related inscriptions
        public List<int> ReviewsIds { get; set; } = new List<int>(); // IDs of related reviews
    }
}
