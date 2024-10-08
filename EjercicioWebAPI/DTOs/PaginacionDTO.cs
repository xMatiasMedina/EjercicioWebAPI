﻿using Microsoft.IdentityModel.Tokens;
using EjercicioWebAPI.Utilidades;

namespace EjercicioWebAPI.DTOs
{
    public class PaginacionDTO
    {
        private const int paginaValorInicial = 1;
        private const int recordsPorPaginaValorInicial = 10;
        //Pagina -> Por defecto siempre sera la 1
        public int Pagina { get; set; } = paginaValorInicial;
        //Campo con la cantidad de records por pagina
        private int recordsPorPagina = recordsPorPaginaValorInicial;
        //Maximo de records por pagina
        private readonly int cantidadMaximaRecordsPorPagina = 50;
    
        public int RecordsPorPagina
        {
            get { return recordsPorPagina; }
            set { recordsPorPagina = (value > cantidadMaximaRecordsPorPagina) ? cantidadMaximaRecordsPorPagina : value; }   
        }

        //Permite vincular un parametro directamente con sus parametros
        public static ValueTask<PaginacionDTO> BindAsync(HttpContext context)
        {
            //var pagina = context.Request.Query[nameof(Pagina)];\
            var pagina = context.ExtraerValorODefecto(nameof(Pagina), paginaValorInicial);
            //var recordsPorPagina = context.Request.Query[nameof(RecordsPorPagina)];
            var recordsPorPagina = context.ExtraerValorODefecto(nameof(RecordsPorPagina), recordsPorPaginaValorInicial);
            /*
            var paginaInt = pagina.IsNullOrEmpty() ? paginaValorInicial : int.Parse(pagina.ToString());
            var recordsPorPaginaInt = recordsPorPagina.IsNullOrEmpty() ? recordsPorPaginaValorInicial : int.Parse(recordsPorPagina.ToString());
            */
            var resultado = new PaginacionDTO
            {
                Pagina = pagina,
                RecordsPorPagina = recordsPorPagina
            };
            return ValueTask.FromResult(resultado);
        }
        public static bool TryParse(string value, out PaginacionDTO paginacionDTO)
        {
            paginacionDTO = null;
            var parts = value.Split(',');

            if (parts.Length != 2)
                return false;

            if (int.TryParse(parts[0], out int pagina) && int.TryParse(parts[1], out int recordsPorPagina))
            {
                paginacionDTO = new PaginacionDTO
                {
                    Pagina = pagina,
                    RecordsPorPagina = recordsPorPagina
                };
                return true;
            }
            return false;
        }
    }
    
}
