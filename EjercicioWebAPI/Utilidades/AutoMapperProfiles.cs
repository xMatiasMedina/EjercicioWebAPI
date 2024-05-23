using AutoMapper;
using EjercicioWebAPI.DTOs;
using EjercicioWebAPI.Entidades;

namespace MinimalAPIPeliculas.Utilidades
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<PreparadorFisico, PreparadorFisicoDTO>();
            CreateMap<CrearPreparadorFisicoDTO, PreparadorFisico>().ForMember(pf => pf.PersonaDNI,
                entidad => entidad.MapFrom(cpf => cpf.crearPersonaDTO.DNI));

            CreateMap<CrearPersonaDTO, Persona>();
        }
    }
}
