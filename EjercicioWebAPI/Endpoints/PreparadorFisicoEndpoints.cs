using AutoMapper;
using EjercicioWebAPI.DTOs;
using EjercicioWebAPI.Entidades;
using EjercicioWebAPI.Repositorios;
using FluentValidation.Internal;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPeliculas.Servicios;
using System.Runtime.CompilerServices;

namespace EjercicioWebAPI.Endpoints
{
    public static class PreparadorFisicoEndpoints
    {
        public static RouteGroupBuilder MapPreparadoresFisicos(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerPreparadoresFisicos).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("pf-get"));
            group.MapGet("/{id:int}", ObtenerPreparadorFisicoPorId);
            group.MapPost("/", CrearPreparadorFisico);
            
            return group;
        }

        static async Task<Ok<List<PreparadorFisicoDTO>>> ObtenerPreparadoresFisicos(IRepositorioPreparadorFisico repositorio, IMapper mapper, ILoggerFactory loggerFactory)
        {
            var preparadoresFisicos = await repositorio.ObtenerTodos();
            var preparadoresFisicosDTO = mapper.Map<List<PreparadorFisicoDTO>>(preparadoresFisicos);
            return TypedResults.Ok(preparadoresFisicosDTO);
        }

        static async Task<Results<Ok<PreparadorFisicoDTO>, NotFound>> ObtenerPreparadorFisicoPorId(int id, IRepositorioPreparadorFisico repositorio, IMapper mapper)
        {
            var preparadorFisico = await repositorio.ObtenerPorId(id);

            if (preparadorFisico is null)
                return TypedResults.NotFound();

            var preparadorFisicoDTO = mapper.Map<PreparadorFisicoDTO>(preparadorFisico);

            return TypedResults.Ok(preparadorFisicoDTO);
        }

        static async Task<Results<ValidationProblem, Created<PreparadorFisicoDTO>, BadRequest<string>>> CrearPreparadorFisico(CrearPreparadorFisicoDTO crearPreparadorFisicoDTO,
            IRepositorioPreparadorFisico repositorioPreparadorFisico, IRepositorioPersona repositorioPersona, IOutputCacheStore outputCacheStore, IMapper mapper
            , IServicioUsuarios servicioUsuarios)
        {
            var preparadorFisico = mapper.Map<PreparadorFisico>(crearPreparadorFisicoDTO);
            var persona = mapper.Map<Persona>(crearPreparadorFisicoDTO.crearPersonaDTO);

            var usuario = await servicioUsuarios.ObtenerUsuario();

            if (usuario is null)
                return TypedResults.BadRequest("Usuario no encontrado");

            persona.UsuarioId = usuario.Id;

            await repositorioPersona.Crear(persona);
            var id = await repositorioPreparadorFisico.Crear(preparadorFisico);
            await outputCacheStore.EvictByTagAsync("pf-get", default);
            var preparadorFisicoDTO = mapper.Map<PreparadorFisicoDTO>(preparadorFisico);
            return TypedResults.Created($"/preparadorfisico/{id}", preparadorFisicoDTO);
            
        }



    }
}
