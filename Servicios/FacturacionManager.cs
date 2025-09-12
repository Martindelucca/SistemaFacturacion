using Practica01.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Practica01.Servicios
{
    public class FacturacionManager
    {
        public FacturaService FacturaService { get; private set; }
        public ArticuloService ArticuloService { get; private set; }
        public FormaPagoService FormaPagoService { get; private set; }

        public FacturacionManager()
        {
            FacturaService = new FacturaService();
            ArticuloService = new ArticuloService();
            FormaPagoService = new FormaPagoService();
        }

        /// <summary>
        /// Proceso completo: Crear factura y agregar varios artículos de una vez
        /// </summary>
        public Factura? ProcesarVentaCompleta(string cliente, int idFormaPago, List<(int idArticulo, int cantidad, decimal? precio)> articulos)
        {
            try
            {
                // 1. Crear factura
                var factura = FacturaService.CrearFactura(cliente, idFormaPago);
                if (factura == null)
                    return null;

                // 2. Agregar todos los artículos
                foreach (var (idArticulo, cantidad, precio) in articulos)
                {
                    if (!FacturaService.AgregarArticuloAFactura(factura, idArticulo, cantidad, precio))
                    {
                        Console.WriteLine($"Error al agregar artículo {idArticulo}");
                        return null;
                    }
                }

                // 3. Guardar factura completa
                if (FacturaService.GuardarFactura(factura))
                {
                    Console.WriteLine($"Venta procesada exitosamente. Factura N°: {factura.NroFactura}");
                    return factura;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en venta completa: {ex.Message}");
                return null;
            }
        }



    }
}
