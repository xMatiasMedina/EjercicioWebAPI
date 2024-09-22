using EjercicioWebAPI.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using EjercicioWebAPI;
using EjercicioWebAPI.DTOs;
using EjercicioWebAPI.Utilidades;

namespace EjercicioWebAPI.Repositorios
{
    public class RepositorioPersona : IRepositorioPersona
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public RepositorioPersona(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            httpContext = httpContextAccessor.HttpContext!;
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

        public async Task<List<Persona>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Personas.AsQueryable(); //Obtengo el queryable de actores
            await httpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable.OrderBy(a => a.Nombre).Paginar(paginacionDTO).ToListAsync();
        }
    }
}
