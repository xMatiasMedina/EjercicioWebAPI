using Microsoft.Extensions.Configuration.UserSecrets;
using EjercicioWebAPI.Entidades;
using Microsoft.AspNetCore.Identity;

namespace EjercicioWebAPI.GraphQL
{
    public class Query
    {
        [Serial] //EF no es compatible con la concurrencia por lo que debe usarse Serial
        [UsePaging] //Permite paginar
        [UseProjection] // Permite proyectar los querys a la base de datos con IQueryable
        [UseFiltering] // para filtrar
        [UseSorting] //Para ordenar
        public IQueryable<Clase> ObtenerClases([Service] ApplicationDbContext context) => context.Clases;

        [Serial] //EF no es compatible con la concurrencia por lo que debe usarse Serial
        [UsePaging] //Permite paginar
        [UseProjection] // Permite proyectar los querys a la base de datos con IQueryable
        [UseFiltering] // para filtrar
        [UseSorting] //Para ordenar
        public IQueryable<PreparadorFisico> ObtenerPreparadoresFisicos([Service] ApplicationDbContext context) => context.PreparadoresFisicos;

        [Serial] //EF no es compatible con la concurrencia por lo que debe usarse Serial
        [UsePaging] //Permite paginar
        [UseProjection] // Permite proyectar los querys a la base de datos con IQueryable
        [UseFiltering] // para filtrar
        [UseSorting] //Para ordenar
        public IQueryable<Review> ObtenerReview([Service] ApplicationDbContext context) => context.Reviews;

        [Serial] //EF no es compatible con la concurrencia por lo que debe usarse Serial
        [UsePaging] //Permite paginar
        [UseProjection] // Permite proyectar los querys a la base de datos con IQueryable
        [UseFiltering] // para filtrar
        [UseSorting] //Para ordenar
        public IQueryable<Inscripcion> ObtenerInscripciones([Service] ApplicationDbContext context) => context.Inscripciones;

        [Serial] //EF no es compatible con la concurrencia por lo que debe usarse Serial
        [UsePaging] //Permite paginar
        [UseProjection] // Permite proyectar los querys a la base de datos con IQueryable
        [UseFiltering] // para filtrar
        [UseSorting] //Para ordenar
        public IQueryable<Persona> ObtenerPersona([Service] ApplicationDbContext context) => context.Personas;

        [Serial] //EF no es compatible con la concurrencia por lo que debe usarse Serial
        [UsePaging] //Permite paginar
        [UseProjection] // Permite proyectar los querys a la base de datos con IQueryable
        [UseFiltering] // para filtrar
        [UseSorting] //Para ordenar
        public IQueryable<IdentityUser> ObtenerUsuarios([Service] ApplicationDbContext context) => context.Users;

    }
}
