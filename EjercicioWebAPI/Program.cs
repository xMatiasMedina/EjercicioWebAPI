var builder = WebApplication.CreateBuilder(args);

//Inicio del area de servicios



// Fin de area de servicios
var app = builder.Build();
// Inicio de area de los middleware
app.MapGet("/", () => "Hello World!");



//Fin de area de los middleware
app.Run();
