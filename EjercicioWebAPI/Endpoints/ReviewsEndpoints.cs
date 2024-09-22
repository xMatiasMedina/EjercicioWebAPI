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
    public static class ReviewsEndpoints
    {
        private static readonly string contenedor = "reviews";
        public static RouteGroupBuilder MapReviews(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("reviews-get"));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPost("/", Crear).DisableAntiforgery()//.AddEndpointFilter<FiltroValidaciones<CrearReviewsDTO>>()
                .RequireAuthorization(policyNames: "esadmin")
                .WithOpenApi();//Desabilita procesos de seguridad (por las imagenes)
            group.MapPut("/{id:int}", Actualizar).DisableAntiforgery()//.AddEndpointFilter<FiltroValidaciones<CrearReviewsDTO>>()
                .RequireAuthorization(policyNames: "esadmin").WithOpenApi();
            group.MapDelete("/{id:int}", Borrar)
                .RequireAuthorization(policyNames: "esadmin");
            return group;
        }

        static async Task<Ok<List<ReviewsDTO>>> ObtenerTodos(IRepositorioReview repositorio, IMapper mapper,
             PaginacionDTO paginacion)//temporal //En el GET No se puede mappear desde el cuerpo
        {
            //var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };
            var reviews = await repositorio.ObtenerTodos(paginacion);
            var reviewsDTO = mapper.Map<List<ReviewsDTO>>(reviews);
            return TypedResults.Ok(reviewsDTO);
        }

        static async Task<Results<Ok<ReviewsDTO>, NotFound>> ObtenerPorId(int id, IRepositorioReview repositorio, IMapper mapper)
        {
            var review = await repositorio.ObtenerPorId(id);

            if (review is null)
            {
                return TypedResults.NotFound();
            }
            var reviewDTO = mapper.Map<ReviewsDTO>(review);
            return TypedResults.Ok(reviewDTO);
        }

        //Para poder recibir el Form File - permite recibir archivos.
        static async Task<Results<ValidationProblem, Created<ReviewsDTO>>> Crear([FromForm] CrearReviewsDTO crearReviewDTO, IRepositorioReview repositorio,
            IOutputCacheStore outputCacheStore, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {

            var review = mapper.Map<Review>(crearReviewDTO);
            var id = await repositorio.Crear(review);
            await outputCacheStore.EvictByTagAsync("reviews-get", default);
            var reviewDTO = mapper.Map<ReviewsDTO>(review);
            return TypedResults.Created($"/reviews/{id}", reviewDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id,
            [FromForm] CrearReviewsDTO crearActorDTO, IRepositorioReview repositorio, IAlmacenadorArchivos almacenadorArchivos,
            IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var reviewDB = await repositorio.ObtenerPorId(id);
            if (reviewDB is null)
            {
                return TypedResults.NotFound();
            }
            var reviewParaActualizar = mapper.Map<Review>(crearActorDTO);
            reviewParaActualizar.Id = id;

            await repositorio.Actualizar(reviewParaActualizar);
            await outputCacheStore.EvictByTagAsync("reviews-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Borrar(int id, IRepositorioReview repositorio,
            IOutputCacheStore outputCacheStore, IAlmacenadorArchivos almacenadorArchivos)
        {
            var actorDB = await repositorio.ObtenerPorId(id);

            if (actorDB is null)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("reviews-get", default);
            return TypedResults.NoContent();
        }
    }
}
