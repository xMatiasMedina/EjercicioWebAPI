using EjercicioWebAPI.Entidades;
using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas;

namespace EjercicioWebAPI.Repositorios
{
    public class RepositorioInscripcion : IRepositorioInscripcion
    {
        private readonly ApplicationDbContext context;

        public RepositorioInscripcion(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task Actualizar(Inscripcion inscripcion)
        {
            context.Update(inscripcion);
            await context.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await context.Inscripciones.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<int> Crear(Inscripcion inscripcion)
        {
            context.Add(inscripcion);
            await context.SaveChangesAsync();
            return inscripcion.Id;
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Inscripciones.AnyAsync(x => x.Id == id);
        }

        public async Task<Inscripcion?> ObtenerPorId(int id)
        {
            return await context.Inscripciones.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Inscripcion>> ObtenerTodos()
        {
            return await context.Inscripciones.ToListAsync();
        }
    }
}

