using Practica01.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica01.Datos.Interfaces
{
    public interface IFacturaRepository
    {
        List<Factura> GetAll();
        Factura GetById(int nroFactura);
        Factura GetFacturaCompleta(int nroFactura);
        bool Save(Factura factura);
        bool Delete(int nroFactura);
        bool AgregarDetalle(int nroFactura, int idArticulo, int cantidad, decimal precioUnitario);
    }
}
