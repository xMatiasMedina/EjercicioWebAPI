using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace EjercicioWebAPI.Entidades
{
    public class Persona
    {
        public int DNI { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;

        public string UsuarioId { get; set; } = null!;
        
        public IdentityUser Usuario { get; set; } = null!;
    }
}
