using System.ComponentModel.DataAnnotations;

namespace Practica01.Dominio
{
    public class FormaPago
    {
        public int IdFormaPago { get; set; }

        [Required(ErrorMessage = "El nombre de la forma de pago es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        public string Nombre { get; set; }

        public FormaPago()
        {
            Nombre = string.Empty;
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
