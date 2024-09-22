using AutoMapper;
using HotChocolate.Authorization;
using EjercicioWebAPI.DTOs;
using EjercicioWebAPI.Entidades;
using EjercicioWebAPI.Repositorios;

namespace EjercicioWebAPI.GraphQL
{
    public class Mutacion
    {
        [Serial]
        [Authorize(Policy = "esadmin")]
        public async Task<ClaseDTO> InsertarClase([Service] IRepositorioClase repositorioClases, [Service] IMapper mapper,
            CrearClaseDTO crearClaseDTO)
        {
            var clase = mapper.Map<Clase>(crearClaseDTO);
            await repositorioClases.Crear(clase);
            var claseDTO = mapper.Map<ClaseDTO>(clase);
            return claseDTO;
        }

        [Serial]
        [Authorize(Policy = "esadmin")]
        public async Task<ClaseDTO?> ActualizarClase([Service] IRepositorioClase repositorioClases, [Service] IMapper mapper,
            CrearClaseDTO actualizarClaseDTO, int id)
        {
            var claseExiste = await repositorioClases.Existe(id);

            if (!claseExiste)
            {
                return null;
            }

            var clase = mapper.Map<Clase>(actualizarClaseDTO);
            clase.Id = id;
            await repositorioClases.Actualizar(clase);
            var claseDTO = mapper.Map<ClaseDTO>(clase);
            return claseDTO;
        }

        [Serial]
        [Authorize(Policy = "esadmin")]
        public async Task<bool> BorrarClase([Service] IRepositorioClase repositorioClases, int id)
        {
            var claseExiste = await repositorioClases.Existe(id);

            if (!claseExiste)
            {
                return false;
            }

            await repositorioClases.Borrar(id);
            return true;
        }
    }
}
