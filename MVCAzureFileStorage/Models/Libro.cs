using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCAzureFileStorage.Models{
    public class Libro{
        public String Nombre { get; set; }

        public String Autor { get; set; }

        public int NumeroPaginas { get; set; }

        public String Imagen { get; set; }

        public List<Capitulo> Capitulos { get; set; }
    }
}