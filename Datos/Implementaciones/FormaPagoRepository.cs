using Practica01.Datos;
using Practica01.Datos.Interfaces;
using Practica01.Dominio;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica01.Datos.Implementaciones
{
    public class FormaPagoRepository : IFormaPagoRepository
    {

        List<FormaPago> formasPago = new List<FormaPago>();

        public List<FormaPago> GetAll()
        {
            throw new NotImplementedException();
            List<FormaPago> formasPago = new List<FormaPago>();

            var dt = DataHelper.GetInstance().ExecuteSPQuery("SP_OBTENER_FORMAS_PAGO");

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    FormaPago formaPago = new FormaPago()
                    {
                        IdFormaPago = (int)row["id_forma_pago"],
                        Nombre = (string)row["nombre"]
                    };
                    formasPago.Add(formaPago);
                }
            }
            return formasPago;
        }

        public FormaPago GetById(int id)
        {
            if (id <= 0) return null;

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@id_forma_pago", SqlDbType.Int) { Value = id }
            };

            // Usamos el SP genérico filtrando por ID
            var dt = DataHelper.GetInstance().ExecuteSPQuery("SP_OBTENER_FORMAS_PAGO");

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if ((int)row["id_forma_pago"] == id)
                    {
                        return new FormaPago()
                        {
                            IdFormaPago = (int)row["id_forma_pago"],
                            Nombre = (string)row["nombre"]
                        };
                    }
                }
            }
            return null;
        }

        public bool Save(FormaPago formaPago)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}

