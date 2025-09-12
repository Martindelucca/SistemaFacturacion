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
    public class FormaPagoService : IFormaPagoService
    {
        private IFormaPagoRepository _repository;

        public FormaPagoService()
        {
            _repository = new FormaPagoRepository();
        }

        /// Constructor para inyección de dependencias (útil para testing)

        public FormaPagoService(IFormaPagoRepository repository)
        {
            _repository = repository;
        }

        /// Obtiene todas las formas de pago disponibles

        public List<FormaPago> ObtenerFormasPago()
        {
            try
            {
                return _repository.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener formas de pago: {ex.Message}");
                return new List<FormaPago>();
            }
        }

        /// <summary>
        /// Obtiene una forma de pago por ID
        /// </summary>
        public FormaPago ObtenerFormaPagoPorId(int idFormaPago)
        {
            try
            {
                if (idFormaPago <= 0)
                    throw new ArgumentException("El ID de forma de pago debe ser mayor a cero");

                return _repository.GetById(idFormaPago);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener forma de pago: {ex.Message}");
                return null;
            }
        }
    }
}
