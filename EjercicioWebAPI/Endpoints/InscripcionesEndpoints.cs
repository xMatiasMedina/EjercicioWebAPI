using AutoMapper;
using EjercicioWebAPI.DTOs;
using EjercicioWebAPI.Entidades;
using EjercicioWebAPI.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using EjercicioWebAPI.DTOs;
using EjercicioWebAPI.Filtros;
using EjercicioWebAPI.Servicios;
using EjercicioWebAPI.Utilidades;

namespace EjercicioWebAPI.Endpoints
{
    public static class InscripcionesEndpoints
    {
        private static readonly string contenedor = "inscripciones";
        public static RouteGroupBuilder MapInscripciones(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("inscripciones-get"));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPost("/", Crear).DisableAntiforgery()//.AddEndpointFilter<FiltroValidaciones<CrearInscripcionesDTO>>()
                .RequireAuthorization(policyNames: "esadmin")
                .WithOpenApi();//Desabilita procesos de seguridad (por las imagenes)
            group.MapPut("/{id:int}", Actualizar).DisableAntiforgery()//.AddEndpointFilter<FiltroValidaciones<CrearInscripcionesDTO>>()
                .RequireAuthorization(policyNames: "esadmin").WithOpenApi();
            group.MapDelete("/{id:int}", Borrar)
                .RequireAuthorization(policyNames: "esadmin");
            return group;
        }

        static async Task<Ok<List<InscripcionesDTO>>> ObtenerTodos(IRepositorioInscripcion repositorio, IMapper mapper,
             PaginacionDTO paginacion)//temporal //En el GET No se puede mappear desde el cuerpo
        {
            //var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };
            var inscripciones = await repositorio.ObtenerTodos(paginacion);
            var inscripcionesDTO = mapper.Map<List<InscripcionesDTO>>(inscripciones);
            return TypedResults.Ok(inscripcionesDTO);
        }

        static async Task<Results<Ok<InscripcionesDTO>, NotFound>> ObtenerPorId(int id, IRepositorioInscripcion repositorio, IMapper mapper)
        {
            var inscripcion = await repositorio.ObtenerPorId(id);

            if (inscripcion is null)
            {
                return TypedResults.NotFound();
            }
            var inscripcionDTO = mapper.Map<InscripcionesDTO>(inscripcion);
            return TypedResults.Ok(inscripcionDTO);
        }

        //Para poder recibir el Form File - permite recibir archivos.
        static async Task<Results<ValidationProblem, Created<InscripcionesDTO>>> Crear([FromForm] CrearInscripcionesDTO crearInscripcionesDTO, IRepositorioInscripcion repositorio,
            IOutputCacheStore outputCacheStore, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {

            var inscripcion = mapper.Map<Inscripcion>(crearInscripcionesDTO);
            var id = await repositorio.Crear(inscripcion);
            await outputCacheStore.EvictByTagAsync("inscripciones-get", default);
            var inscripcionDTO = mapper.Map<InscripcionesDTO>(inscripcion);
            return TypedResults.Created($"/actores/{id}", inscripcionDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id,
            [FromForm] CrearInscripcionesDTO crearInscripcionDTO, IRepositorioInscripcion repositorio, IAlmacenadorArchivos almacenadorArchivos,
            IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var inscripcionesDB = await repositorio.ObtenerPorId(id);
            if (inscripcionesDB is null)
            {
                return TypedResults.NotFound();
            }
            var inscripcionesParaActualizar = mapper.Map<Inscripcion>(crearInscripcionDTO);
            inscripcionesParaActualizar.Id = id;
            await repositorio.Actualizar(inscripcionesParaActualizar);
            await outputCacheStore.EvictByTagAsync("inscripciones-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Borrar(int id, IRepositorioInscripcion repositorio,
            IOutputCacheStore outputCacheStore, IAlmacenadorArchivos almacenadorArchivos)
        {
            var inscripcionesDB = await repositorio.ObtenerPorId(id);

            if (inscripcionesDB is null)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("inscripciones-get", default);
            return TypedResults.NoContent();
        }
    }
}
