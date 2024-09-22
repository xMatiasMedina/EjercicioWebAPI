using EjercicioWebAPI.Repositorios;
using FluentValidation;
using EjercicioWebAPI;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using EjercicioWebAPI;
using EjercicioWebAPI.Endpoints;
using EjercicioWebAPI.Entidades;
using EjercicioWebAPI.GraphQL;
using EjercicioWebAPI.Repositorios;
using EjercicioWebAPI.Servicios;
using EjercicioWebAPI.Swagger;
using EjercicioWebAPI.Utilidades;
using Error = EjercicioWebAPI.Entidades.Error;
using EjercicioWebAPI.GraphQL;

var builder = WebApplication.CreateBuilder(args);
var apellido = builder.Configuration.GetValue<string>("apellido");
var origenesPermitidos = builder.Configuration.GetValue<string>("origenesPermitidos")!;
//Inicio de area de los servicios

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
opciones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddGraphQLServer().AddQueryType<Query>()
    .AddMutationType<Mutacion>()
    .AddAuthorization()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

//De esta manera estamos configurando Identity para utilizarlo con nuestra instancia del DbContext
builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<IdentityUser>>(); //representa a un usuario de nuestra aplicacion.

builder.Services.AddScoped<SignInManager<IdentityUser>>();

builder.Services.AddCors(opciones => //Cross origin resource sharing
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
//builder.Services.AddOutputCache();

//Output cache con redis
builder.Services.AddStackExchangeRedisOutputCache(opciones =>
{
    opciones.Configuration = builder.Configuration.GetConnectionString("redis");
});

builder.Services.AddEndpointsApiExplorer(); //Le permite a swagger explorar y listar los endpoints
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v0.1", new OpenApiInfo
    {
        Title = "Ejercicio API",
        Description = "Este es un web api para trabajar con clases de ejercicio",
        Contact = new OpenApiContact
        {
            Email = "anonimo",
            Name = "anonimo",
            Url = new Uri("-")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/license/mit/")
        }
    });
    //Para permitir usar los endpoints que requieren autorizacion
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.OperationFilter<FiltroAutorizacion>();
    /*
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, new string[]{}
        }
    });
    */
});

//Configurar mi propio servicio. Siempre que quiera usar el rep Generos se lo llama por su abstraccion.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IRepositorioClase, RepositorioClase>();
builder.Services.AddScoped<IRepositorioInscripcion, RepositorioInscripcion>();
builder.Services.AddScoped<IRepositorioPersona, RepositorioPersona>();
builder.Services.AddScoped<IRepositorioPreparadorFisico, RepositorioPreparadorFisico>();
builder.Services.AddScoped<IRepositorioErrores, RepositorioErrores>();


//AZURE
//builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosAzure>();
//Almacenar en Archivos
builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();

//SCOPED VS TRANSIENT: TRANSIENT IS ALWAYS DIFFERENT AND SCOPED ARE THE SAME WITHIN REQUESTS
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();

//AutoMapper
builder.Services.AddAutoMapper(typeof(Program));//Utilice como base el proyecto en el cual se encuetra la clase Program.

//Fluent para validaciones
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

//Permite agregar mensajes/mostrar informacion sobre el manejo de errores
builder.Services.AddProblemDetails();


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

//Fin de area de los servicios
var app = builder.Build();
//Inicio del area de los middleware

if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Solo con esta implementacion ya se muestran mas detalles sobre el error.
app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
{
    var exceptionHandleFeature = context.Features.Get<IExceptionHandlerFeature>();
    var excepcion = exceptionHandleFeature?.Error!;

    var error = new Error();
    error.Fecha = DateTime.UtcNow;
    error.MensajeDeError = excepcion.Message;
    error.StackTrace = excepcion.StackTrace;

    var repositorio = context.RequestServices.GetRequiredService<IRepositorioErrores>(); //Repositorio de Errores
    await repositorio.Crear(error); //Se guarda el error en la base de datos

    await TypedResults.BadRequest(new { tipo = "error", mensaje = "ha ocurrido un mensaje de error inesperado", estatus = 500 })
    .ExecuteAsync(context);
}));

//Permite manejar excepciones, de momento lo unico que hace es ocultar el mensaje de error
app.UseStatusCodePages(); //Permite configurar nuestra app para que retorne codigos de status cuando haya un error

app.UseStaticFiles(); //Dado que el wwwroot se colocan archivos estaticos.

app.UseCors();

app.UseOutputCache();

//Uso el servicio de Autorizacion
app.UseAuthorization();

//GraphQL
app.MapGraphQL();

//Tomar datos de distintas fuentes
// [FromQuery(Name = "nombre2")] [FromBody] [FromHeader(Name = "nombre")]
app.MapPost("/modelbinding/{nombre}", ([FromRoute] string? nombre) =>
{
    if (nombre is null)
    {
        nombre = "Vacio";
    }

    return TypedResults.Ok(nombre);
});

app.MapGet("/", [EnableCors(policyName: "libre")] () => apellido); //Que ocurre con un Get en un endpoint
app.MapGet("/error", () =>
{
    throw new InvalidOperationException("error de ejemplo");
});

app.MapGroup("/usuarios").MapUsuarios();
app.MapGroup("/preparadoresFisicos").MapPreparadoresFisicos();
app.MapGroup("/clases/{claseId:int}/reviews").MapReviews();
app.MapGroup("/personas").MapPersonas();
app.MapGroup("/inscripciones").MapInscripciones();
app.MapGroup("/clases").MapClases();



//app.MapGroup("/usuarios").MapUsuarios();
//Fin de area de los middleware
app.Run();

