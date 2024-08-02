using AutoMapper;
using EjercicioWebAPI.DTOs;
using EjercicioWebAPI.Entidades;
using EjercicioWebAPI.Repositorios;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Filtros;
using MinimalAPIPeliculas.Servicios;
using MinimalAPIPeliculas.Utilidades;

namespace EjercicioWebAPI.Endpoints
{
    public static class PersonasEndpoints
    {
        private static readonly string contenedor = "actores";
        public static RouteGroupBuilder MapActores(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("actores-get"))
                .AgregarParametrosPaginacionAOpenAPI(); //Con esto puedo modificar la meta data del endpoint para que swagger sepa que mostrar.

            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPost("/", Crear).DisableAntiforgery().AddEndpointFilter<FiltroValidaciones<CrearPersonaDTO>>()
                .RequireAuthorization(policyNames: "esadmin")
                .WithOpenApi();//Desabilita procesos de seguridad (por las imagenes)
            group.MapPut("/{id:int}", Actualizar).DisableAntiforgery().AddEndpointFilter<FiltroValidaciones<CrearPersonaDTO>>()
                .RequireAuthorization(policyNames: "esadmin").WithOpenApi();
            group.MapDelete("/{id:int}", Borrar)
                .RequireAuthorization(policyNames: "esadmin");
            return group;
        }

        static async Task<Ok<List<PersonaDTO>>> ObtenerTodos(IRepositorioPersona repositorio, IMapper mapper,
             PaginacionDTO paginacion)//temporal //En el GET No se puede mappear desde el cuerpo
        {
            //var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };
            var actores = await repositorio.ObtenerTodos(paginacion);
            var actoresDTO = mapper.Map<List<PersonaDTO>>(actores);
            return TypedResults.Ok(actoresDTO);
        }

        static async Task<Results<Ok<PersonaDTO>, NotFound>> ObtenerPorId(int dni, IRepositorioPersona repositorio, IMapper mapper)
        {
            var persona = await repositorio.ObtenerPorDNI(dni);

            if (persona is null)
            {
                return TypedResults.NotFound();
            }
            var actorDTO = mapper.Map<PersonaDTO>(persona);
            return TypedResults.Ok(actorDTO);
        }

        //Para poder recibir el Form File - permite recibir archivos.
        static async Task<Results<ValidationProblem, Created<PersonaDTO>>> Crear([FromForm] CrearPersonaDTO crearPersonaDTO, IRepositorioPersona repositorio,
            IOutputCacheStore outputCacheStore, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            var persona = mapper.Map<Persona>(crearPersonaDTO);

            var dni = await repositorio.Crear(persona);
            await outputCacheStore.EvictByTagAsync("persona-get", default);
            var personaDTO = mapper.Map<PersonaDTO>(persona);
            return TypedResults.Created($"/personas/{dni}", personaDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int dni,
            [FromForm] CrearPersonaDTO crearPersonaDTO, IRepositorioPersona repositorio, IAlmacenadorArchivos almacenadorArchivos,
            IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var actorDB = await repositorio.ObtenerPorDNI(dni);
            if (actorDB is null)
            {
                return TypedResults.NotFound();
            }
            var personaParaActualizar = mapper.Map<Persona>(crearPersonaDTO);
            personaParaActualizar.DNI = dni;
            await repositorio.Actualizar(personaParaActualizar);
            await outputCacheStore.EvictByTagAsync("persona-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Borrar(int dni, IRepositorioPersona repositorio,
            IOutputCacheStore outputCacheStore, IAlmacenadorArchivos almacenadorArchivos)
        {
            var personaDB = await repositorio.ObtenerPorDNI(dni);

            if (personaDB is null)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(dni);
            await outputCacheStore.EvictByTagAsync("persona-get", default);
            return TypedResults.NoContent();
        }
    }
}
