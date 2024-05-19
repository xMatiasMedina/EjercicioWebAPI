using EjercicioWebAPI.Entidades;
using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas;

namespace EjercicioWebAPI.Repositorios
{
    public class RepositorioReview : IRepositorioReview
    {
        private readonly ApplicationDbContext context;

        public RepositorioReview(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task Actualizar(Review review)
        {
            context.Update(review);
            await context.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await context.Reviews.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<int> Crear(Review review)
        {
            context.Add(review);
            await context.SaveChangesAsync();
            return review.Id;
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Reviews.AnyAsync(x => x.Id == id);
        }

        public async Task<Review?> ObtenerPorId(int id)
        {
            return await context.Reviews.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Review>> ObtenerTodos()
        {
            return await context.Reviews.ToListAsync();
        }
    }
}
