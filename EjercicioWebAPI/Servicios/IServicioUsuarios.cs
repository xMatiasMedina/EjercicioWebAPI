﻿using Microsoft.AspNetCore.Identity;

namespace EjercicioWebAPI.Servicios
{
    public interface IServicioUsuarios
    {
        Task<IdentityUser?> ObtenerUsuario();

    }
}