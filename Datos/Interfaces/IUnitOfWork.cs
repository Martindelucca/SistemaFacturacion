using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Practica01.Datos.Interfaces
{
    public interface IUnitOfWork : IDisposable  
    {
        IFacturaRepository Facturas { get; }
        IArticuloRepository Articulos { get; }
        IFormaPagoRepository FormasPago { get; }

        void BeginTransaction();
        bool SaveChanges();
        void Rollback();
    }
}
