using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AxisCRM.Api.DTO
{
    public class ModelErrorDTO
    {
        public string Titulo { get; set; }
        public int StatusCode { get; set; }
        public string Mensagem { get; set; }

        public ModelErrorDTO(string titulo, int statusCode, string mensagem)
        {
            Titulo = titulo;
            StatusCode = statusCode;
            Mensagem = mensagem;
        }
    }
}