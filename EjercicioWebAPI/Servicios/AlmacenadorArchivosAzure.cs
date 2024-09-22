/*
 * Conexion con AZURE, usa un paquete llamado AZURE BLOBS
 * 
 */
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Path = System.IO.Path;

namespace EjercicioWebAPI.Servicios
{
    public class AlmacenadorArchivosAzure : IAlmacenadorArchivos
    {

        private string connectionString;

        public AlmacenadorArchivosAzure(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("AzureStorage")!;
        }

        public async Task<string> Almacenar(string contenedor, IFormFile archivo)
        {
            var cliente = new BlobContainerClient(connectionString, contenedor);
            await cliente.CreateIfNotExistsAsync();///Si ya existe no hace nada
            cliente.SetAccessPolicy(PublicAccessType.Blob);//Quiero tener acceso al blob
            var extension = Path.GetExtension(archivo.FileName);//extension del archivo
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var blob = cliente.GetBlobClient(nombreArchivo);
            var blobHttpHeaders = new BlobHttpHeaders();
            blobHttpHeaders.ContentType = archivo.ContentType;
            await blob.UploadAsync(archivo.OpenReadStream(), blobHttpHeaders);
            return blob.Uri.ToString();
        }

        public async Task Borrar(string? ruta, string contenedor)
        {
            if (string.IsNullOrEmpty(ruta))
            {
                return;
            }
            var cliente = new BlobContainerClient(connectionString, contenedor);
            await cliente.CreateIfNotExistsAsync();///Si ya existe no hace nada
            var nombreArchivo = Path.GetFileName(ruta);
            var blob = cliente.GetBlobClient(nombreArchivo);
            await blob.DeleteIfExistsAsync(); 
        }
    }
}
