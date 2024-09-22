using AutoMapper;
using EjercicioWebAPI.DTOs;
using EjercicioWebAPI.Entidades;
using EjercicioWebAPI.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using EjercicioWebAPI.Filtros;
using EjercicioWebAPI.Servicios;
using EjercicioWebAPI.Utilidades;

namespace EjercicioWebAPI.Endpoints
{
    public static class PreparadorFisicoEndpoints
    {
        private static readonly string contenedor = "preparadoresfisicos";
        public static RouteGroupBuilder MapPreparadoresFisicos(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("actores-get"));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPost("/", Crear).DisableAntiforgery()//.AddEndpointFilter<FiltroValidaciones<CrearPreparadorFisicoDTO>>()
                .RequireAuthorization(policyNames: "esadmin")
                .WithOpenApi();//Desabilita procesos de seguridad (por las imagenes)
            group.MapPut("/{id:int}", Actualizar).DisableAntiforgery()//.AddEndpointFilter<FiltroValidaciones<CrearPreparadorFisicoDTO>>()
                .RequireAuthorization(policyNames: "esadmin").WithOpenApi();
            group.MapDelete("/{id:int}", Borrar)
                .RequireAuthorization(policyNames: "esadmin");
            return group;
        }

        static async Task<Ok<List<PreparadorFisicoDTO>>> ObtenerTodos(IRepositorioPreparadorFisico repositorio, IMapper mapper,
             PaginacionDTO paginacion)//temporal //En el GET No se puede mappear desde el cuerpo
        {
            //var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };
            var preparadoresFisicos = await repositorio.ObtenerTodos(paginacion);
            var preparadoresFisicosDTO = mapper.Map<List<PreparadorFisicoDTO>>(preparadoresFisicos);
            return TypedResults.Ok(preparadoresFisicosDTO);
        }

        static async Task<Results<Ok<PreparadorFisicoDTO>, NotFound>> ObtenerPorId(int id, IRepositorioPreparadorFisico repositorio, IMapper mapper)
        {
            var preparadorFisico = await repositorio.ObtenerPorId(id);

            if (preparadorFisico is null)
            {
                return TypedResults.NotFound();
            }
            var preparadorFisicoDTO = mapper.Map<PreparadorFisicoDTO>(preparadorFisico);
            return TypedResults.Ok(preparadorFisicoDTO);
        }

        //Para poder recibir el Form File - permite recibir archivos.
        static async Task<Results<ValidationProblem, Created<PreparadorFisicoDTO>>> Crear([FromForm] CrearPreparadorFisicoDTO crearPreparadorFisicoDTO, IRepositorioPreparadorFisico repositorio,
            IOutputCacheStore outputCacheStore, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {

            var preparadorFisico = mapper.Map<PreparadorFisico>(crearPreparadorFisicoDTO);


            var id = await repositorio.Crear(preparadorFisico);
            await outputCacheStore.EvictByTagAsync("pfisico-get", default);
            var preparadorFisicoDTO = mapper.Map<PreparadorFisicoDTO>(preparadorFisico);
            return TypedResults.Created($"/preparadoresfisicos/{id}", preparadorFisicoDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id,
            [FromForm] CrearPreparadorFisicoDTO crearPreparadorFisicoDTO, IRepositorioPreparadorFisico repositorio, IAlmacenadorArchivos almacenadorArchivos,
            IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var preparadorFisicoDB = await repositorio.ObtenerPorId(id);
            if (preparadorFisicoDB is null)
            {
                return TypedResults.NotFound();
            }
            var preparadorFisicoParaActualizar = mapper.Map<PreparadorFisico>(crearPreparadorFisicoDTO);
            preparadorFisicoParaActualizar.Id = id;
            await repositorio.Actualizar(preparadorFisicoParaActualizar);
            await outputCacheStore.EvictByTagAsync("pfisico-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Borrar(int id, IRepositorioPreparadorFisico repositorio,
            IOutputCacheStore outputCacheStore, IAlmacenadorArchivos almacenadorArchivos)
        {
            var preparadorFisicoDB = await repositorio.ObtenerPorId(id);

            if (preparadorFisicoDB is null)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("pfisico-get", default);
            return TypedResults.NoContent();
        }
    }
}
