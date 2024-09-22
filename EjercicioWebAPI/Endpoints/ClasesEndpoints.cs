using AutoMapper;
using EjercicioWebAPI.DTOs;
using EjercicioWebAPI.Entidades;
using EjercicioWebAPI.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using EjercicioWebAPI.DTOs;
using EjercicioWebAPI.Filtros;
using EjercicioWebAPI.Utilidades;

namespace EjercicioWebAPI.Endpoints
{
    public static class ClasesEndpoints
    {
        private static readonly string contenedor = "clases";

        public static RouteGroupBuilder MapClases(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("clases-get"));

            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPost("/", Crear).DisableAntiforgery().AddEndpointFilter<FiltroValidaciones<CrearClaseDTO>>()
                .RequireAuthorization("esadmin","espfisico")
                .WithOpenApi();
            group.MapPut("/{id:int}", Actualizar).DisableAntiforgery().AddEndpointFilter<FiltroValidaciones<CrearClaseDTO>>()
                .RequireAuthorization("esadmin","espfisico").WithOpenApi();
            group.MapDelete("/{id:int}", Borrar)
                .RequireAuthorization("esadmin", "espfisico");

            return group;
        }

        static async Task<Ok<List<ClaseDTO>>> ObtenerTodos(IRepositorioClase repositorio, IMapper mapper, PaginacionDTO paginacion)
        {
            var clases = await repositorio.ObtenerTodos(paginacion);
            var clasesDTO = mapper.Map<List<ClaseDTO>>(clases);
            return TypedResults.Ok(clasesDTO);
        }

        static async Task<Results<Ok<ClaseDTO>, NotFound>> ObtenerPorId(int id, IRepositorioClase repositorio, IMapper mapper)
        {
            var clase = await repositorio.ObtenerPorId(id);

            if (clase is null)
            {
                return TypedResults.NotFound();
            }

            var claseDTO = mapper.Map<ClaseDTO>(clase);
            return TypedResults.Ok(claseDTO);
        }

        static async Task<Results<ValidationProblem, Created<ClaseDTO>>> Crear([FromForm] CrearClaseDTO crearClaseDTO, IRepositorioClase repositorio, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var clase = mapper.Map<Clase>(crearClaseDTO);
            var id = await repositorio.Crear(clase);
            await outputCacheStore.EvictByTagAsync("clases-get", default);
            var claseDTO = mapper.Map<ClaseDTO>(clase);
            return TypedResults.Created($"/clases/{id}", claseDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id, [FromForm] CrearClaseDTO crearClaseDTO, IRepositorioClase repositorio, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var claseDB = await repositorio.ObtenerPorId(id);

            if (claseDB is null)
            {
                return TypedResults.NotFound();
            }

            var claseParaActualizar = mapper.Map<Clase>(crearClaseDTO);
            claseParaActualizar.Id = id;
            await repositorio.Actualizar(claseParaActualizar);
            await outputCacheStore.EvictByTagAsync("clases-get", default);

            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Borrar(int id, IRepositorioClase repositorio, IOutputCacheStore outputCacheStore)
        {
            var claseDB = await repositorio.ObtenerPorId(id);

            if (claseDB is null)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("clases-get", default);
            return TypedResults.NoContent();
        }
    }
}
