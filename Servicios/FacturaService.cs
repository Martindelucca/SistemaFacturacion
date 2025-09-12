using Practica01.Datos.Implementaciones;
using Practica01.Datos.Interfaces;
using Practica01.Dominio;
using Practica01.Servicios.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Practica01.Servicios
{
    public class FacturaService : IFacturaService
    {
        private IFacturaRepository _facturaRepository;
        private IArticuloRepository _articuloRepository;
        private IFormaPagoRepository _formaPagoRepository;

        public FacturaService()
        {
            _facturaRepository = new FacturaRepository();
            _articuloRepository = new ArticuloRepository();
            _formaPagoRepository = new FormaPagoRepository();
        }

        public FacturaService(IFacturaRepository facturaRepository,
                          IArticuloRepository articuloRepository,
                          IFormaPagoRepository formaPagoRepository)
        {
            _facturaRepository = facturaRepository;
            _articuloRepository = articuloRepository;
            _formaPagoRepository = formaPagoRepository;
        }


        /// Obtiene todas las facturas

        public List<Factura> ObtenerFacturas()
        {
            try
            {
                return _facturaRepository.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener facturas: {ex.Message}");
                return new List<Factura>();
            }
        }

        /// <summary>
        /// Obtiene una factura completa con todos sus detalles
        /// </summary>
        public Factura? ObtenerFacturaCompleta(int nroFactura)
        {
            try
            {
                if (nroFactura <= 0)
                    throw new ArgumentException("El número de factura debe ser mayor a cero");

                return _facturaRepository.GetFacturaCompleta(nroFactura);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener factura completa: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Crea una nueva factura vacía
        /// </summary>
        public Factura CrearFactura(string cliente, int idFormaPago, DateTime? fecha = null)
        {
            try
            {
                // Validaciones de negocio
                if (string.IsNullOrWhiteSpace(cliente))
                    throw new ArgumentException("El nombre del cliente es obligatorio");

                if (idFormaPago <= 0)
                    throw new ArgumentException("Debe seleccionar una forma de pago válida");

                // Verificar que la forma de pago existe
                var formaPago = _formaPagoRepository.GetById(idFormaPago);
                if (formaPago == null)
                    throw new ArgumentException("La forma de pago seleccionada no existe");

                // Crear la factura
                var factura = new Factura()
                {
                    Cliente = cliente.Trim(),
                    IdFormaPago = idFormaPago,
                    Fecha = fecha ?? DateTime.Now,
                    FormaPago = formaPago
                };

                return factura;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear factura: {ex.Message}");
                throw; // Re-lanzamos para que el controlador maneje el error
            }
        }

        /// <summary>
        /// Agrega un artículo a una factura existente
        /// MANEJA AUTOMÁTICAMENTE LA REGLA DE ARTÍCULOS DUPLICADOS
        /// </summary>
        public bool AgregarArticuloAFactura(Factura factura, int idArticulo, int cantidad, decimal? precioEspecial = null)
        {
            try
            {
                // Validaciones
                if (factura == null)
                    throw new ArgumentException("La factura no puede ser nula");

                if (idArticulo <= 0)
                    throw new ArgumentException("Debe seleccionar un artículo válido");

                if (cantidad <= 0)
                    throw new ArgumentException("La cantidad debe ser mayor a cero");

                // Obtener el artículo
                var articulo = _articuloRepository.GetById(idArticulo);
                if (articulo == null)
                    throw new ArgumentException("El artículo seleccionado no existe");

                // Usar precio especial o precio actual del artículo
                decimal precioUnitario = precioEspecial ?? articulo.PrecioUnitario;

                // Agregar a la factura (la entidad maneja automáticamente los duplicados)
                factura.AgregarDetalle(articulo, cantidad, precioUnitario);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar artículo: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Guarda una factura completa en la base de datos
        /// </summary>
        public bool GuardarFactura(Factura factura)
        {
            try
            {
                // Validaciones finales antes de guardar
                if (factura == null)
                    throw new ArgumentException("La factura no puede ser nula");

                if (!factura.TieneDetalles())
                    throw new ArgumentException("La factura debe tener al menos un detalle");

                if (string.IsNullOrWhiteSpace(factura.Cliente))
                    throw new ArgumentException("El cliente es obligatorio");

                // Recalcular total por seguridad
                factura.RecalcularTotal();

                // Guardar usando el repositorio (que maneja las transacciones)
                bool resultado = _facturaRepository.Save(factura);

                if (resultado)
                {
                    Console.WriteLine($"Factura guardada exitosamente. Número: {factura.NroFactura}");
                }

                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar factura: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Agrega un detalle a una factura YA EXISTENTE en base de datos
        /// Útil para agregar artículos después de crear la factura
        /// </summary>
        public bool AgregarDetalleAFacturaExistente(int nroFactura, int idArticulo, int cantidad, decimal? precioEspecial = null)
        {
            try
            {
                // Validaciones
                if (nroFactura <= 0)
                    throw new ArgumentException("Número de factura inválido");

                var articulo = _articuloRepository.GetById(idArticulo);
                if (articulo == null)
                    throw new ArgumentException("Artículo no encontrado");

                decimal precioUnitario = precioEspecial ?? articulo.PrecioUnitario;

                // Usar el método del repositorio que maneja automáticamente los duplicados
                return _facturaRepository.AgregarDetalle(nroFactura, idArticulo, cantidad, precioUnitario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar detalle: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina una factura (eliminación lógica)
        /// </summary>
        public bool EliminarFactura(int nroFactura)
        {
            try
            {
                if (nroFactura <= 0)
                    throw new ArgumentException("Número de factura inválido");

                return _facturaRepository.Delete(nroFactura);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar factura: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene las facturas de un cliente específico
        /// </summary>
        public List<Factura> ObtenerFacturasPorCliente(string cliente)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cliente))
                    return new List<Factura>();

                var todasLasFacturas = _facturaRepository.GetAll();
                return todasLasFacturas
                    .Where(f => f.Cliente.ToLower().Contains(cliente.ToLower().Trim()))
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar facturas por cliente: {ex.Message}");
                return new List<Factura>();
            }
        }

        /// <summary>
        /// Obtiene estadísticas básicas de facturación
        /// </summary>
        public object? ObtenerEstadisticas()
        {
            try
            {
                var facturas = _facturaRepository.GetAll();

                return new
                {
                    TotalFacturas = facturas.Count,
                    MontoTotalFacturado = facturas.Sum(f => f.Total),
                    PromedioFactura = facturas.Count > 0 ? facturas.Average(f => f.Total) : 0,
                    FacturasPorMes = facturas
                        .GroupBy(f => new { f.Fecha.Year, f.Fecha.Month })
                        .Select(g => new
                        {
                            Periodo = $"{g.Key.Year}-{g.Key.Month:00}",
                            Cantidad = g.Count(),
                            Monto = g.Sum(f => f.Total)
                        })
                        .OrderBy(x => x.Periodo)
                        .ToList()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener estadísticas: {ex.Message}");
                return null;
            }
        }
    }
}
