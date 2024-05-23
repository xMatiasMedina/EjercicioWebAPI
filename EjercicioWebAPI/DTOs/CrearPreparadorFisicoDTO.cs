using EjercicioWebAPI.Entidades;

namespace EjercicioWebAPI.DTOs
{
    public class CrearPreparadorFisicoDTO
    {
        public CrearPersonaDTO crearPersonaDTO { get; set; }
        public string CBU { get; set; } = null!;
    }
}
