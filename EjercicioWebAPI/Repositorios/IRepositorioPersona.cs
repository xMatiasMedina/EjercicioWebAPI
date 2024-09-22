using EjercicioWebAPI.Entidades;
using EjercicioWebAPI.DTOs;

namespace EjercicioWebAPI.Repositorios
{
    public interface IRepositorioPersona
    {
        Task Actualizar(Persona persona);
        Task Borrar(int dni);
        Task<int> Crear(Persona persona);
        Task<bool> Existe(int dni);
        Task<Persona?> ObtenerPorDNI(int dni);
        Task<List<Persona>> ObtenerTodos(PaginacionDTO paginacionDTO);
    }
}