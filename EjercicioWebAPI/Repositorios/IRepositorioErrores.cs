using EjercicioWebAPI.Entidades;
using Error = EjercicioWebAPI.Entidades.Error; //ALIAS
namespace EjercicioWebAPI.Repositorios
{
    public interface IRepositorioErrores
    {
        Task Crear(Error error);
    }
}