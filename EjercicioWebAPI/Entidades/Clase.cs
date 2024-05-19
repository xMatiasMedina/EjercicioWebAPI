using Microsoft.AspNetCore.Routing.Constraints;

namespace EjercicioWebAPI.Entidades
{
    public class Clase
    {
        public int Id { get; set; }
        public string Actividad { get; set; } = null!;
        public int PreparadorFisicoId { get; set; }
        public DateTime DiaHorario { get; set; }
        public string Ubicacion { get; set; } = null!;
        public float PuntajePromedio { get; set; }
        public float Precio {  get; set; }
        public int CupoMax { get; set; }

        public PreparadorFisico PreparadorFisico { get; set; } = null!;
        public List<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}
