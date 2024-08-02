using EjercicioWebAPI.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Utilidades;

namespace EjercicioWebAPI.Repositorios
{
    public class RepositorioReview : IRepositorioReview
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public RepositorioReview(ApplicationDbContext context, HttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContext = httpContextAccessor.HttpContext!;
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

        public async Task<List<Review>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Reviews.AsQueryable(); //Obtengo el queryable de actores
            await httpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable.OrderBy(a => a.Id).Paginar(paginacionDTO).ToListAsync();
        }
    }
}
