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

//Servicio de Validaciones (Fluent)
builder.Services.AddValidatorsFromAssemblyContaining<Program>();


// Fin de area de servicios
var app = builder.Build();
// Inicio de area de los middleware
app.MapGet("/", () => "Hello World!");

app.UseOutputCache();

app.UseAuthorization();

//Fin de area de los middleware
app.Run();

