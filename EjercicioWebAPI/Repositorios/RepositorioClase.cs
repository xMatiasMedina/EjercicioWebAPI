using EjercicioWebAPI.Entidades;
using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas;

namespace EjercicioWebAPI.Repositorios
{
    public class RepositorioClase : IRepositorioClase
    {
        private readonly ApplicationDbContext context;

        public RepositorioClase(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task Actualizar(Clase clase)
        {
            context.Update(clase);
            await context.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await context.Clases.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<int> Crear(Clase clase)
        {
            context.Add(clase);
            await context.SaveChangesAsync();
            return clase.Id;
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Clases.AnyAsync(x => x.Id == id);
        }

        public async Task<Clase?> ObtenerPorId(int id)
        {
            return await context.Clases
                .Include(x => x.PreparadorFisico)
                .Include(x => x.Inscripciones)
                .Include(x => x.Reviews)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Clase>> ObtenerTodos()
        {
            return await context.Clases.Include(p => p.PreparadorFisico).ToListAsync();
        }
    }
}

