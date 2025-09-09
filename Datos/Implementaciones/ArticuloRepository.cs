using Practica01.Datos.Interfaces;
using Practica01.Dominio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica01.Datos.Implementaciones
{
    public class ArticuloRepository : IArticuloRepository
    {
        public List<Articulo> GetAll()
        {
            List<Articulo> articulos = new List<Articulo>();

            var dt = DataHelper.GetInstance().ExecuteSPQuery("SP_OBTENER_ARTICULOS");

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Articulo articulo = new Articulo()
                    {
                        IdArticulo = (int)row["id_articulo"],
                        Nombre = (string)row["nombre"],
                        PrecioUnitario = (decimal)row["precio_unitario"],
                    };
                    articulos.Add(articulo);
                }
            }
            return articulos;
        }

        public Articulo GetById(int id)
        {
            if (id <= 0) return null;

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@id_articulo", SqlDbType.Int) { Value = id }
            };

            var dt = DataHelper.GetInstance().ExecuteSPNonQuery("SP_OBTENER_ARTICULO_POR_ID", parametros);

            if (dt == null || dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];

            return new Articulo()
            {
                IdArticulo = (int)row["id_articulo"],
                Nombre = (string)row["nombre"],
                PrecioUnitario = (decimal)row["precio_unitario"],
            };
        }

        public bool Save(Articulo articulo)
        {
            // Por ahora solo implemento para uso básico
            // Se puede extender para crear/actualizar artículos
            throw new NotImplementedException("Funcionalidad no implementada aún");
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException("Funcionalidad no implementada aún");
        }
    }
}

