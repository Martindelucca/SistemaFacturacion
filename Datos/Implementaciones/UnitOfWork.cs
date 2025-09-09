using Practica01.Datos.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica01.Datos.Implementaciones
{
    public class UnitOfWork
    {
        private IFacturaRepository? _facturaRepository;
        private IArticuloRepository? _articuloRepository;
        private IFormaPagoRepository? _formaPagoRepository;
        private SqlTransaction _transaction;
        private SqlConnection? _connection;
        private bool _disposed = false;

        public IFacturaRepository Facturas
        {
            get
            {
                if (_facturaRepository == null)
                    _facturaRepository = new FacturaRepository();
                return _facturaRepository;
            }
        }

        public IArticuloRepository Articulos
        {
            get
            {
                if (_articuloRepository == null)
                    _articuloRepository = new ArticuloRepository();
                return _articuloRepository;
            }
        }

        public IFormaPagoRepository FormasPago
        {
            get
            {
                if (_formaPagoRepository == null)
                    _formaPagoRepository = new FormaPagoRepository();
                return _formaPagoRepository;
            }
        }

        public void BeginTransaction()
        {
            try
            {
                _connection = DataHelper.GetInstance().GetConnection();
                _connection.Open();
                _transaction = _connection.BeginTransaction();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al iniciar transacción: " + ex.Message);
                throw;
            }
        }

        public bool SaveChanges()
        {
            try
            {
                if (_transaction != null)
                {
                    _transaction.Commit();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al confirmar transacción: " + ex.Message);
                Rollback();
                return false;
            }
        }

        public void Rollback()
        {
            try
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al hacer rollback: " + ex.Message);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }

                if (_connection != null && _connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
