using EjercicioWebAPI.Entidades;
using MinimalAPIPeliculas.DTOs;

namespace EjercicioWebAPI.Repositorios
{
    public interface IRepositorioReview
    {
        Task Actualizar(Review review);
        Task Borrar(int id);
        Task<int> Crear(Review review);
        Task<bool> Existe(int id);
        Task<Review?> ObtenerPorId(int id);
        Task<List<Review>> ObtenerTodos(PaginacionDTO paginacionDTO);
    }
}