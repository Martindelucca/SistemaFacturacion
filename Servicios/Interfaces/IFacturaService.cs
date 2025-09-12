using Practica01.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica01.Servicios.Interfaces
{
    public interface IFacturaService
    {
        List<Factura> ObtenerFacturas();
        Factura ObtenerFacturaCompleta(int nroFactura);
        Factura CrearFactura(string cliente, int idFormaPago, DateTime? fecha = null);
        bool AgregarArticuloAFactura(Factura factura, int idArticulo, int cantidad, decimal? precioEspecial = null);
        bool GuardarFactura(Factura factura);
        bool AgregarDetalleAFacturaExistente(int nroFactura, int idArticulo, int cantidad, decimal? precioEspecial = null);
        bool EliminarFactura(int nroFactura);
        List<Factura> ObtenerFacturasPorCliente(string cliente);
        object ObtenerEstadisticas();
    }
}
