using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica01.Dominio
{
    public class Articulo
    {
        public int IdArticulo { get; set; }

        [Required(ErrorMessage = "El nombre del artículo es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El precio unitario es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero")]
        public decimal PrecioUnitario { get; set; }
      
        public Articulo()
        {
            Nombre = string.Empty;
            PrecioUnitario = 0;
        }

        public override string ToString()
        {
            return $"{Nombre} - ${PrecioUnitario:F2}";
        }
    }
}
