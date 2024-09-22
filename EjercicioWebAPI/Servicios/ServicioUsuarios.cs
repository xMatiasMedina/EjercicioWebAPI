using Microsoft.AspNetCore.Identity;

namespace EjercicioWebAPI.Servicios
{
    public class ServicioUsuarios : IServicioUsuarios
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<IdentityUser> userManager;

        public ServicioUsuarios(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        {
            var emailClaim = httpContextAccessor.HttpContext!
                .User.Claims.Where(x => x.Type == "email").FirstOrDefault(); //ASPNETCORE hace mapeos automaticos a ciertos tipos y les cambia el nombre

            if (emailClaim is null)
            {
                return null;
            }
            var email = emailClaim.Value;
            return await userManager.FindByEmailAsync(email);
        }
    }
}
