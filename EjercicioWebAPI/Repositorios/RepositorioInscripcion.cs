using EjercicioWebAPI.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Utilidades;

namespace EjercicioWebAPI.Repositorios
{
    public class RepositorioInscripcion : IRepositorioInscripcion
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public RepositorioInscripcion(ApplicationDbContext context, HttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContext = httpContextAccessor.HttpContext!;
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

        public async Task<List<Inscripcion>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Inscripciones.AsQueryable(); //Obtengo el queryable de actores
            await httpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable.OrderBy(a => a.Id).Paginar(paginacionDTO).ToListAsync();
        }
    }
}

