using EjercicioWebAPI.Repositorios;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas;
using MinimalAPIPeliculas.Repositorios;
using MinimalAPIPeliculas.Servicios;
using MinimalAPIPeliculas.Utilidades;
using MinimalAPIPeliculas.Entidades;

var builder = WebApplication.CreateBuilder(args);
var origenesPermitidos = builder.Configuration.GetValue<string>("origenesPermitidos")!;
//Inicio del area de servicios

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
opciones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<IdentityUser>>();
builder.Services.AddScoped<UserManager<IdentityUser>>();

// DEJO EL PLANTEO EN CASO DE QUERER USARSE CORS.
/* 
builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(configuracion =>
    {
        configuracion.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    });

    opciones.AddPolicy("libre", configuracion =>
    {
        configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});
*/

builder.Services.AddOutputCache();
builder.Services.AddHttpContextAccessor();

//TODO: SWAGGER INFO

//Repositorios de Entidades
builder.Services.AddScoped<IRepositorioPersona, RepositorioPersona>();
builder.Services.AddScoped<IRepositorioPreparadorFisico, RepositorioPreparadorFisico>();
builder.Services.AddScoped<IRepositorioClase, RepositorioClase>();
builder.Services.AddScoped<IRepositorioInscripcion, RepositorioInscripcion>();
builder.Services.AddScoped<IRepositorioReview, RepositorioReview>();
builder.Services.AddScoped<IRepositorioErrores, RepositorioErrores>();

//Servicio de Usuarios que implementa Identity
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();

//Servicio de Mapeo entre clases
builder.Services.AddAutoMapper(typeof(Program));

//Servicio de Validaciones (Fluent)
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

//Autenticacion y Autorizacion
builder.Services.AddAuthentication().AddJwtBearer(opciones =>
{
    opciones.MapInboundClaims = false; //ES PARA QUE NO HAGA UN MAPEO SOBRE LOS CLAIMS QUE TERMINA CAMBIANDO LOS NOMBRES
    opciones.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters

    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true, //Tiempo de vencimiento para los tokens
        ValidateIssuerSigningKey = true,
        //IssuerSigningKey = Llaves.ObtenerLlave(builder.Configuration).First() //Para una unica llave y nos utilizamos a nosotros mismos como emisores de los tokens
        IssuerSigningKeys = Llaves.ObtenerTodasLasLlaves(builder.Configuration), //Para multiples llaves
        ClockSkew = TimeSpan.Zero//Evita problemas de tiempo a la hora de evaluar si el token a vencido o no
    };
});

builder.Services.AddAuthorization(opciones =>
{
    opciones.AddPolicy("esadmin", politica => politica.RequireClaim("esadmin"));
});

// Fin de area de servicios
var app = builder.Build();
// Inicio de area de los middleware

if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
{
    var exceptionHandleFeature = context.Features.Get<IExceptionHandlerFeature>();
    var excepcion = exceptionHandleFeature?.Error!;

    var error = new MinimalAPIPeliculas.Entidades.Error();
    error.Fecha = DateTime.UtcNow;
    error.MensajeDeError = excepcion.Message;
    error.StackTrace = excepcion.StackTrace;

    var repositorio = context.RequestServices.GetRequiredService<IRepositorioErrores>(); //Repositorio de Errores
    await repositorio.Crear(error); //Se guarda el error en la base de datos

    await TypedResults.BadRequest(new { tipo = "error", mensaje = "ha ocurrido un mensaje de error inesperado", estatus = 500 })
    .ExecuteAsync(context);
}));

//app.UseCors();

app.UseOutputCache();

app.UseAuthorization();

//Fin de area de los middleware
app.Run();
