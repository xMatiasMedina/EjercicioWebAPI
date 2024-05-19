using EjercicioWebAPI.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Error = MinimalAPIPeliculas.Entidades.Error;

namespace MinimalAPIPeliculas
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Configuracion por Fluent
            //Persona
            modelBuilder.Entity<Persona>().HasKey(p => p.DNI);
            modelBuilder.Entity<Persona>().Property(p => p.Nombre).HasMaxLength(50);
            modelBuilder.Entity<Persona>().Property(p => p.Apellido).HasMaxLength(50);

            //PreparadorFisico
            modelBuilder.Entity<PreparadorFisico>().Property(p => p.CBU).HasMaxLength(22); //Se guarda solo los caracters que corresponder con una URL. + eficiente

            //Clase
            modelBuilder.Entity<Clase>().Property(p => p.Actividad).HasMaxLength(150);
            modelBuilder.Entity<Clase>().Property(p => p.Ubicacion).HasMaxLength(200);
            
            //Tablas de Usuarios
            modelBuilder.Entity<IdentityUser>().ToTable("Usuarios");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RolesClaims");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UsuariosClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UsuariosLogins");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UsuariosRoles");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UsuariosTokens");
        }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<PreparadorFisico> PreparadoresFisicos { get; set; }
        public DbSet<Clase> Clases { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<Error> Errores { get; set; }
    }
}
