using EjercicioWebAPI.Entidades;

namespace EjercicioWebAPI.Repositorios
{
    public interface IRepositorioInscripcion
    {
        Task Actualizar(Inscripcion inscripcion);
        Task Borrar(int id);
        Task<int> Crear(Inscripcion inscripcion);
        Task<bool> Existe(int id);
        Task<Inscripcion?> ObtenerPorId(int id);
        Task<List<Inscripcion>> ObtenerTodos();
    }
}