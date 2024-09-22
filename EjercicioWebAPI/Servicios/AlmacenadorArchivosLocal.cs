
using System.Reflection.Metadata.Ecma335;
using Path = System.IO.Path;

namespace EjercicioWebAPI.Servicios
{
    public class AlmacenadorArchivosLocal : IAlmacenadorArchivos
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        /*
* IWebHostEnviroment: Me va a permitir obtener la direccion donde se encuetra
* la carpeta en la cual yo quiero guardar las imagenes.
* IHttpContextAccesor: Me va a dar acceso al contexto HTTP.
*/
        public AlmacenadorArchivosLocal(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> Almacenar(string contenedor, IFormFile archivo)
        {
            var extension = Path.GetExtension(archivo.FileName);
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            //WebRootPath: Se refiere a una carpeta especial que podemos tener en ASP.NET llamada wwwroot.
            //wwwroot: Es una carpeta a traves de la cual puedo compartir archivos en la aplicacion
            string folder = Path.Combine(env.WebRootPath, contenedor);//Contenedor es para segmentar las carpetas
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            //ruta del archivo, combino nombre con archivo
            string ruta = Path.Combine(folder, nombreArchivo);
            using (var ms = new MemoryStream()) //Para escribir el archivo en el directorio
            {
                await archivo.CopyToAsync(ms); //Copiamos de manera asincrona al memory stream
                var contenido = ms.ToArray(); //Contenido del archivo como arreglo de bytes
                await File.WriteAllBytesAsync(ruta, contenido); //Se escribe en el archivo
            }
            /*
             * CREO LA URL
             * Scheme: Se refiere a si es http o https
             * Host: Se refiere al url actual, como miapi.com
             */
            var url = $"{httpContextAccessor.HttpContext!.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            //Se remplaza \\ con / porque en windows y OS  en general se usa \\ pero en url en general se usa /
            var urlArchivo = Path.Combine(url, contenedor, nombreArchivo).Replace("\\", "/");
            return urlArchivo;
        }

        public Task Borrar(string? ruta, string contenedor)
        {
            if (string.IsNullOrEmpty(ruta))
            {
                return Task.CompletedTask;
            }

            var nombreArchivo = Path.GetFileName(ruta);
            var directorioArchivo = Path.Combine(env.WebRootPath, contenedor, nombreArchivo);
            
            if (File.Exists(directorioArchivo))
            {
                File.Delete(directorioArchivo);
            }
            return Task.CompletedTask;
        }
    }
}
