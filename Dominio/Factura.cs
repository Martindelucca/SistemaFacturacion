using System.ComponentModel.DataAnnotations;

namespace Practica01.Dominio
{
    public class Factura
    {
        public int NroFactura { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "La forma de pago es obligatoria")]
        public int  IdFormaPago { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre del cliente no puede exceder los 100 caracteres")]
        public string Cliente { get; set; }

        public decimal Total { get; set; }

        // Propiedades de navegación
        public FormaPago FormaPago { get; set; }
        public List<DetalleFactura> Detalles { get; set; }

        public Factura()
        {
            Fecha = DateTime.Now;
            Cliente = string.Empty;
            Total = 0;
            Detalles = new List<DetalleFactura>();
            FormaPago = new FormaPago();
        }
        public void AgregarDetalle(Articulo articulo, int cantidad, decimal? precioUnitario = null)
        {
            if (articulo == null)
                throw new ArgumentNullException(nameof(articulo), "El artículo no puede ser nulo");

            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero", nameof(cantidad));

            // Usar el precio actual del artículo si no se especifica uno
            decimal precio = precioUnitario ?? articulo.PrecioUnitario;

            // Buscar si ya existe el artículo en los detalles
            var detalleExistente = Detalles.Find(d => d.IdArticulo == articulo.IdArticulo);

            if (detalleExistente != null)
            {
                // Si existe, sumar las cantidades (regla de negocio)
                detalleExistente.Cantidad += cantidad;
                detalleExistente.PrecioUnitario = precio; // Actualizar con el precio más reciente
            }
            else
            {
                // Si no existe, crear nuevo detalle
                var nuevoDetalle = new DetalleFactura
                {
                    NroFactura = this.NroFactura,
                    IdArticulo = articulo.IdArticulo,
                    Cantidad = cantidad,
                    PrecioUnitario = precio,
                    Articulo = articulo
                };
                Detalles.Add(nuevoDetalle);
            }

            // Recalcular total
            RecalcularTotal();
        }
        /// </summary>
        public void RecalcularTotal()
        {
            Total = 0;
            foreach (var detalle in Detalles)
            {
                Total += detalle.Subtotal;
            }
        }

        /// <summary>
        /// Elimina un detalle de la factura
        /// </summary>
        public bool EliminarDetalle(int idArticulo)
        {
            var detalle = Detalles.Find(d => d.IdArticulo == idArticulo);
            if (detalle != null)
            {
                Detalles.Remove(detalle);
                RecalcularTotal();
                return true;
            }
            return false;
        }

        public int CantidadTotalArticulos()
        {
            int total = 0;
            foreach (var detalle in Detalles)
            {
                total += detalle.Cantidad;
            }
            return total;
        }


        /// Verifica si la factura tiene detalles

        public bool TieneDetalles()
        {
            return Detalles.Count > 0;
        }

        /// Obtiene una copia de solo lectura de los detalles


        public override string ToString()
        {
            return $"Factura {NroFactura} - {Cliente} - ${Total:F2}";
        }


    }
}
