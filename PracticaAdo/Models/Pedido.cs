﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaAdo.Models
{
    public class Pedido
    {
        public string CodigoPedido { get; set; }
        public string CodigoCliente { get; set; }
        public string FechaEntrega { get; set; }
        public string FormaEnvio { get; set; }
        public string Importe { get; set; }
    }
}
