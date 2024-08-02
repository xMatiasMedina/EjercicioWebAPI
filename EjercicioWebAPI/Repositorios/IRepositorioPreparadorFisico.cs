using EjercicioWebAPI.Entidades;
using MinimalAPIPeliculas.DTOs;

namespace EjercicioWebAPI.Repositorios
{
    public interface IRepositorioPreparadorFisico
    {
        Task Actualizar(PreparadorFisico preparador);
        Task Borrar(int id);
        Task<int> Crear(PreparadorFisico preparador);
        Task<bool> Existe(int id);
        Task<PreparadorFisico?> ObtenerPorId(int id);
        Task<List<PreparadorFisico>> ObtenerTodos(PaginacionDTO paginacionDTO);
    }
}