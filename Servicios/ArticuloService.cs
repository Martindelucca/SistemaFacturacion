using Practica01.Datos.Implementaciones;
using Practica01.Datos.Interfaces;
using Practica01.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Practica01.Servicios
{
    public class ArticuloService
    {
        private IArticuloRepository _repository;

        public ArticuloService()
        {
            _repository = new ArticuloRepository();
        }

        /// <summary>
        /// Obtiene todos los artículos disponibles
        /// </summary>
        public List<Articulo> ObtenerArticulos()
        {
            try
            {
                return _repository.GetAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener artículos: {ex.Message}");
                return new List<Articulo>();
            }
        }

        /// <summary>
        /// Obtiene un artículo por su ID
        /// </summary>
        public Articulo ObtenerArticuloPorId(int idArticulo)
        {
            try
            {
                if (idArticulo <= 0)
                    throw new ArgumentException("El ID del artículo debe ser mayor a cero");

                return _repository.GetById(idArticulo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener artículo: {ex.Message}");
                return null;
            }
        }

        }
}
