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
    public static class ReviewsEndpoints
    {
        private static readonly string contenedor = "reviews";

        public static RouteGroupBuilder MapReviews(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("reviews-get"));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPost("/", Crear)
                .RequireAuthorization("esadmin")
                .WithOpenApi();
            group.MapPut("/{id:int}", Actualizar)
                .RequireAuthorization("esadmin")
                .WithOpenApi();
            group.MapDelete("/{id:int}", Borrar)
                .RequireAuthorization("esadmin");
            return group;
        }

        static async Task<Results<Ok<List<ReviewsDTO>>, NotFound>> ObtenerTodos(
            [FromServices] IRepositorioReview repositorio,
            [FromServices] IMapper mapper,
            [FromQuery] PaginacionDTO paginacion) // Use [FromQuery] for pagination
        {
            var reviews = await repositorio.ObtenerTodos(paginacion);
            var reviewsDTO = mapper.Map<List<ReviewsDTO>>(reviews);
            return TypedResults.Ok(reviewsDTO);
        }

        static async Task<Results<Ok<ReviewsDTO>, NotFound>> ObtenerPorId(
            int id,
            [FromServices] IRepositorioReview repositorio,
            [FromServices] IMapper mapper)
        {
            var review = await repositorio.ObtenerPorId(id);
            if (review is null)
            {
                return TypedResults.NotFound();
            }
            var reviewDTO = mapper.Map<ReviewsDTO>(review);
            return TypedResults.Ok(reviewDTO);
        }

        static async Task<Results<ValidationProblem, Created<ReviewsDTO>>> Crear(
            [FromBody] CrearReviewsDTO crearReviewDTO, // Changed to [FromBody]
            [FromServices] IRepositorioReview repositorio,
            [FromServices] IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper,
            [FromServices] IAlmacenadorArchivos almacenadorArchivos)
        {
            var review = mapper.Map<Review>(crearReviewDTO);
            var id = await repositorio.Crear(review);
            await outputCacheStore.EvictByTagAsync("reviews-get", default);
            var reviewDTO = mapper.Map<ReviewsDTO>(review);
            return TypedResults.Created($"/reviews/{id}", reviewDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(
            int id,
            [FromBody] CrearReviewsDTO crearReviewDTO, // Changed to [FromBody]
            [FromServices] IRepositorioReview repositorio,
            [FromServices] IAlmacenadorArchivos almacenadorArchivos,
            [FromServices] IOutputCacheStore outputCacheStore,
            [FromServices] IMapper mapper)
        {
            var reviewDB = await repositorio.ObtenerPorId(id);
            if (reviewDB is null)
            {
                return TypedResults.NotFound();
            }
            var reviewParaActualizar = mapper.Map<Review>(crearReviewDTO);
            reviewParaActualizar.Id = id;

            await repositorio.Actualizar(reviewParaActualizar);
            await outputCacheStore.EvictByTagAsync("reviews-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Borrar(
            int id,
            [FromServices] IRepositorioReview repositorio,
            [FromServices] IOutputCacheStore outputCacheStore,
            [FromServices] IAlmacenadorArchivos almacenadorArchivos)
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
