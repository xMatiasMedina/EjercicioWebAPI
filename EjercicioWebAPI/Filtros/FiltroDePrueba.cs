
using AutoMapper;
using EjercicioWebAPI.Repositorios;
using EjercicioWebAPI.Repositorios;

namespace EjercicioWebAPI.Filtros
{
    public class FiltroDePrueba : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            // Este codigo se ejecuta antes del endpoint
            var paramRepositorio = context.Arguments.OfType<IRepositorioClase>().FirstOrDefault();
            var paramEntero = context.Arguments.OfType<int>().FirstOrDefault();
            var paramMapper = context.Arguments.OfType<IMapper>().FirstOrDefault();
            var resultado = await next(context);
            //Este codigo se ejecuta despues del enpoint
            return resultado;
        }
    }
}
