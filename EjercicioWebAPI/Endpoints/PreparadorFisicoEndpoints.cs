using AutoMapper;
using EjercicioWebAPI.DTOs;
using EjercicioWebAPI.Entidades;
using EjercicioWebAPI.Repositorios;
using FluentValidation.Internal;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace EjercicioWebAPI.Endpoints
{
    public static class PreparadorFisicoEndpoints
    {
        private static readonly string contenedor = "preparadoresfisicos";
        public static RouteGroupBuilder MapPreparadoresFisicos(this RouteGroupBuilder group)
        {
            return group;
        }

        {
            var preparadoresFisicosDTO = mapper.Map<List<PreparadorFisicoDTO>>(preparadoresFisicos);
            return TypedResults.Ok(preparadoresFisicosDTO);
        }

        {
            var preparadorFisico = await repositorio.ObtenerPorId(id);

            if (preparadorFisico is null)
            {
                return TypedResults.NotFound();
            var preparadorFisicoDTO = mapper.Map<PreparadorFisicoDTO>(preparadorFisico);

            return TypedResults.Ok(preparadorFisicoDTO);
        }

        {

            var preparadorFisico = mapper.Map<PreparadorFisico>(crearPreparadorFisicoDTO);
            var persona = mapper.Map<Persona>(crearPreparadorFisicoDTO.crearPersonaDTO);

            var usuario = await servicioUsuarios.ObtenerUsuario();

            if (usuario is null)
                return TypedResults.BadRequest("Usuario no encontrado");

            persona.UsuarioId = usuario.Id;

            var preparadorFisicoDTO = mapper.Map<PreparadorFisicoDTO>(preparadorFisico);
            
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
