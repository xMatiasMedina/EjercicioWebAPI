using Microsoft.EntityFrameworkCore;

namespace EjercicioWebAPI.Utilidades
{
    public static class HttpContextExtensions
    {
        /*
         * Le dice a los clientes la cantidad de paginas a traves de la cabecera de http
         * IQueryable: Es propio de EF, y nos permite hacer operaciones sobre el mismo sin
         * conocer la tabla sobre la cual se trabajara,
         */
        public async static Task InsertarParametrosPaginacionEnCabecera<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            if (httpContext is null) { throw new ArgumentNullException(nameof(httpContext)); }

            double cantidad = await queryable.CountAsync();//Cuenta los registros que hay
            //Se agrega a cabecera la cantidad de registros
            httpContext.Response.Headers.Append("cantidadTotalRegistros", cantidad.ToString());
        }
    }
}
