using EjercicioWebAPI.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using EjercicioWebAPI;
using EjercicioWebAPI.DTOs;
using EjercicioWebAPI.Utilidades;

namespace EjercicioWebAPI.Repositorios
{
    public class RepositorioClase : IRepositorioClase
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public RepositorioClase(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContext = httpContextAccessor.HttpContext!;
            
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

        public async Task<List<Clase>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Clases.Include(p => p.PreparadorFisico).AsQueryable(); //Obtengo el queryable de actores
            await httpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable.OrderBy(a => a.DiaHorario).Paginar(paginacionDTO).ToListAsync();
        }
    }
}

