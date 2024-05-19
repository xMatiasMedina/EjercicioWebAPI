using EjercicioWebAPI.Entidades;
using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas;

namespace EjercicioWebAPI.Repositorios
{
    public class RepositorioPersona : IRepositorioPersona
    {
        private readonly ApplicationDbContext context;

        public RepositorioPersona(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task Actualizar(Persona persona)
        {
            context.Update(persona);
            await context.SaveChangesAsync();
        }

        public async Task Borrar(int dni)
        {
            await context.Personas.Where(x => x.DNI == dni).ExecuteDeleteAsync();
        }

        public async Task<int> Crear(Persona persona)
        {
            context.Add(persona);
            await context.SaveChangesAsync();
            return persona.DNI;
        }

        public async Task<bool> Existe(int dni)
        {
            return await context.Personas.AnyAsync(x => x.DNI == dni);
        }

        public async Task<Persona?> ObtenerPorDNI(int dni)
        {
            return await context.Personas.FirstOrDefaultAsync(x => x.DNI == dni);
        }

        public async Task<List<Persona>> ObtenerTodos()
        {
            return await context.Personas.ToListAsync();
        }
    }
}
