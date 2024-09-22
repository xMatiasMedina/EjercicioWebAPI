namespace EjercicioWebAPI.DTOs
{
    public class CrearClaseDTO
    {
        public string Actividad { get; set; } = string.Empty;
        public int PreparadorFisicoId { get; set; }
        public DateTime DiaHorario { get; set; }
        public string Ubicacion { get; set; } = string.Empty;
        public float PuntajePromedio { get; set; }
        public float Precio { get; set; }
        public int CupoMax { get; set; }
    }
}
