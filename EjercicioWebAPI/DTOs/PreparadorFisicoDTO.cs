using EjercicioWebAPI.Entidades;

namespace EjercicioWebAPI.DTOs
{
    public class PreparadorFisicoDTO
    {
        public int Id { get; set; }
        public int PersonaDNI { get; set; } //DNI Preparador Fisico
        public string CBU { get; set; } = null!;
        public float PuntajeClasePromedio { get; set; }
        public Persona Persona { get; set; } = null!;//Propiedad de Navegacion
        public List<Clase> Clases { get; set; } = new List<Clase>();

    }
}
