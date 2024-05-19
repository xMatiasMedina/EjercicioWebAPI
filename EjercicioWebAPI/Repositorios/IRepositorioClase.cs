using EjercicioWebAPI.Entidades;

namespace EjercicioWebAPI.Repositorios
{
    public interface IRepositorioClase
    {
        Task Actualizar(Clase clase);
        Task Borrar(int id);
        Task<int> Crear(Clase clase);
        Task<bool> Existe(int id);
        Task<Clase?> ObtenerPorId(int id);
        Task<List<Clase>> ObtenerTodos();
    }
}