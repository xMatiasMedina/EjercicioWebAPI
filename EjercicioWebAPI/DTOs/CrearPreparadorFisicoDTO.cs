using EjercicioWebAPI.Entidades;

namespace EjercicioWebAPI.DTOs
{
    public class CrearPreparadorFisicoDTO
    {
        public int PersonaDNI { get; set; } //DNI Preparador Fisico
        public string CBU { get; set; } = null!;
    }
}
