using EjercicioWebAPI.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Utilidades;

namespace EjercicioWebAPI.Repositorios
{
    public class RepositorioPreparadorFisico : IRepositorioPreparadorFisico
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public RepositorioPreparadorFisico(ApplicationDbContext context, HttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task Actualizar(PreparadorFisico preparador)
        {
            context.Update(preparador);
            await context.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await context.PreparadoresFisicos.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<int> Crear(PreparadorFisico preparador)
        {
            context.Add(preparador);
            await context.SaveChangesAsync();
            return preparador.Id;
        }

        public async Task<bool> Existe(int id)
        {
            return await context.PreparadoresFisicos.AnyAsync(x => x.Id == id);
        }

        public async Task<PreparadorFisico?> ObtenerPorId(int id)
        {
            return await context.PreparadoresFisicos
                .Include(p => p.Persona)
                .Include(p => p.Clases)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<PreparadorFisico>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = context.PreparadoresFisicos
                .Include(p => p.Persona)
                .Include(p => p.Clases)
                .AsQueryable(); //Obtengo el queryable de actores
            await httpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable.OrderBy(a => a.Id).Paginar(paginacionDTO).ToListAsync();
        }
    }
}
