using System.ComponentModel.DataAnnotations;

namespace Practica01.Dominio
{
    public class DetalleFactura
    {
        public int IdDetalle { get; set; }

        public int NroFactura { get; set; }

        [Required(ErrorMessage = "El artículo es obligatorio")]
        public int IdArticulo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El precio unitario es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero")]
        public decimal PrecioUnitario { get; set; }

        /// <summary>
        /// Subtotal calculado (cantidad * precio unitario)
        /// </summary>
        public decimal Subtotal
        {
            get { return Cantidad * PrecioUnitario; }
        }

        // Propiedades de navegación
        public Articulo Articulo { get; set; }

        public DetalleFactura()
        {
            Cantidad = 1;
            PrecioUnitario = 0;
            Articulo = new Articulo();
        }

        /// <summary>
        /// Actualiza la cantidad del detalle
        /// </summary>
        public void ActualizarCantidad(int nuevaCantidad)
        {
            if (nuevaCantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero", nameof(nuevaCantidad));

            Cantidad = nuevaCantidad;
        }

        /// <summary>
        /// Actualiza el precio unitario del detalle
        /// </summary>
        public void ActualizarPrecio(decimal nuevoPrecio)
        {
            if (nuevoPrecio <= 0)
                throw new ArgumentException("El precio debe ser mayor a cero", nameof(nuevoPrecio));

            PrecioUnitario = nuevoPrecio;
        }

        public override string ToString()
        {
            return $"{Articulo?.Nombre} x{Cantidad} - ${Subtotal:F2}";
        }
    }
}