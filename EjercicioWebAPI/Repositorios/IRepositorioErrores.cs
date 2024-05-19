using MinimalAPIPeliculas.Entidades;
using Error = MinimalAPIPeliculas.Entidades.Error; //ALIAS
namespace MinimalAPIPeliculas.Repositorios
{
    public interface IRepositorioErrores
    {
        Task Crear(Error error);
    }
}