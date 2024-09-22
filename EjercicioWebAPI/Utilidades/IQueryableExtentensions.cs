using EjercicioWebAPI.DTOs;

namespace EjercicioWebAPI.Utilidades
{
    public static class IQueryableExtentensions
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDTO paginacionDTO)
        {
            return queryable.Skip((paginacionDTO.Pagina - 1) * paginacionDTO.RecordsPorPagina)//Cuantos registros saltamos?
                .Take(paginacionDTO.RecordsPorPagina);
        }
    }
}
