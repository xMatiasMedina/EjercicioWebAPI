using AutoMapper;
using EjercicioWebAPI.DTOs;
using EjercicioWebAPI.Entidades;
using EjercicioWebAPI.DTOs;
using EjercicioWebAPI.Entidades;

namespace EjercicioWebAPI.Utilidades
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CrearClaseDTO, Clase>();
            CreateMap<Clase, ClaseDTO>();
            CreateMap<CrearInscripcionesDTO, Inscripcion>();
            CreateMap<Inscripcion, InscripcionesDTO>();
            CreateMap<CrearPreparadorFisicoDTO, PreparadorFisico>();
            CreateMap<PreparadorFisico, PreparadorFisicoDTO>();
            CreateMap<CrearReviewsDTO, Review>();
            CreateMap<Review, ReviewsDTO>();
            CreateMap<CrearPersonaDTO, Persona>();
            CreateMap<Persona, PersonaDTO>();
        }
    }
}
