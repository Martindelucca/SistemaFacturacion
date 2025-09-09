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
    public class FacturaRepository : IFacturaRepository
    {
        public List<Factura> GetAll()
        {
            List<Factura> facturas = new List<Factura>();

            var dt = DataHelper.GetInstance().ExecuteSPQuery("SP_OBTENER_FACTURAS");

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Factura factura = new Factura()
                    {
                        NroFactura = (int)row["nro_factura"],
                        Fecha = (DateTime)row["fecha"],
                        Cliente = (string)row["cliente"],
                        Total = (decimal)row["total"],
                        FormaPago = new FormaPago()
                        {
                            Nombre = (string)row["forma_pago"]
                        }
                    };
                    facturas.Add(factura);
                }
            }
            return facturas;
        }

        public Factura GetById(int nroFactura)
        {
            if (nroFactura <= 0) return null;

            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@nro_factura", SqlDbType.Int) { Value = nroFactura }
            };

            var dt = DataHelper.GetInstance().ExecuteSPNonQuery("SP_OBTENER_FACTURA_COMPLETA", parametros);

            if (dt == null || dt.Rows.Count == 0) return null;

            // El SP devuelve 2 result sets, pero con ExecuteSPNonQuery solo obtenemos el primero
            // Para obtener la factura completa, usar GetFacturaCompleta
            DataRow row = dt.Rows[0];

            return new Factura()
            {
                NroFactura = (int)row["nro_factura"],
                Fecha = (DateTime)row["fecha"],
                Cliente = (string)row["cliente"],
                Total = (decimal)row["total"],
                IdFormaPago = (int)row["id_forma_pago"],
                FormaPago = new FormaPago()
                {
                    IdFormaPago = (int)row["id_forma_pago"],
                    Nombre = (string)row["forma_pago"]
                }
            };
        }

        public Factura GetFacturaCompleta(int nroFactura)
        {
            if (nroFactura <= 0) return null;

            Factura factura = null;
            SqlConnection cnn = null;

            try
            {
                cnn = DataHelper.GetInstance().GetConnection();
                cnn.Open();

                using (var cmd = new SqlCommand("SP_OBTENER_FACTURA_COMPLETA", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nro_factura", nroFactura);

                    using (var reader = cmd.ExecuteReader())
                    {
                        // Primer result set: datos de la factura
                        if (reader.Read())
                        {
                            factura = new Factura()
                            {
                                NroFactura = (int)reader["nro_factura"],
                                Fecha = (DateTime)reader["fecha"],
                                Cliente = (string)reader["cliente"],
                                Total = (decimal)reader["total"],
                                IdFormaPago = (int)reader["id_forma_pago"],
                                FormaPago = new FormaPago()
                                {
                                    IdFormaPago = (int)reader["id_forma_pago"],
                                    Nombre = (string)reader["forma_pago"]
                                }
                            };
                        }

                        // Segundo result set: detalles de la factura
                        if (reader.NextResult() && factura != null)
                        {
                            while (reader.Read())
                            {
                                var detalle = new DetalleFactura()
                                {
                                    IdDetalle = (int)reader["id_detalle"],
                                    NroFactura = nroFactura,
                                    IdArticulo = (int)reader["id_articulo"],
                                    Cantidad = (int)reader["cantidad"],
                                    PrecioUnitario = (decimal)reader["precio_unitario"],
                                    Articulo = new Articulo()
                                    {
                                        IdArticulo = (int)reader["id_articulo"],
                                        Nombre = (string)reader["nombre_articulo"]
                                    }
                                };
                                factura.Detalles.Add(detalle);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error en DB: " + ex.Message);
                factura = null;
            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open)
                    cnn.Close();
            }

            return factura;
        }

        public bool Save(Factura factura)
        {
            bool result = true;
            SqlTransaction t = null;
            SqlConnection cnn = null;

            try
            {
                cnn = DataHelper.GetInstance().GetConnection();
                cnn.Open();
                t = cnn.BeginTransaction();

                // Insertar factura (maestro)
                var cmdFactura = new SqlCommand("SP_INSERTAR_FACTURA", cnn, t);
                cmdFactura.CommandType = CommandType.StoredProcedure;

                // Parámetros de entrada
                cmdFactura.Parameters.AddWithValue("@fecha", factura.Fecha);
                cmdFactura.Parameters.AddWithValue("@id_forma_pago", factura.IdFormaPago);
                cmdFactura.Parameters.AddWithValue("@cliente", factura.Cliente);

                // Parámetro de salida
                SqlParameter paramNroFactura = new SqlParameter("@nro_factura", SqlDbType.Int);
                paramNroFactura.Direction = ParameterDirection.Output;
                cmdFactura.Parameters.Add(paramNroFactura);

                cmdFactura.ExecuteNonQuery();

                int nroFactura = (int)paramNroFactura.Value;
                factura.NroFactura = nroFactura;

                // Insertar detalles
                foreach (var detalle in factura.Detalles)
                {
                    var cmdDetalle = new SqlCommand("SP_INSERTAR_DETALLE_FACTURA", cnn, t);
                    cmdDetalle.CommandType = CommandType.StoredProcedure;
                    cmdDetalle.Parameters.AddWithValue("@nro_factura", nroFactura);
                    cmdDetalle.Parameters.AddWithValue("@id_articulo", detalle.IdArticulo);
                    cmdDetalle.Parameters.AddWithValue("@cantidad", detalle.Cantidad);
                    cmdDetalle.Parameters.AddWithValue("@precio_unitario", detalle.PrecioUnitario);

                    cmdDetalle.ExecuteNonQuery();
                }

                t.Commit();
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error en DB: " + ex.Message);
                if (t != null)
                {
                    t.Rollback();
                }
                result = false;
            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }

            return result;
        }

        public bool AgregarDetalle(int nroFactura, int idArticulo, int cantidad, decimal precioUnitario)
        {
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@nro_factura", SqlDbType.Int) { Value = nroFactura },
                new SqlParameter("@id_articulo", SqlDbType.Int) { Value = idArticulo },
                new SqlParameter("@cantidad", SqlDbType.Int) { Value = cantidad },
                new SqlParameter("@precio_unitario", SqlDbType.Decimal) { Value = precioUnitario }
            };

            return DataHelper.GetInstance().ExecuteSPInsertUpdate("SP_INSERTAR_DETALLE_FACTURA", parametros);
        }

        public bool Delete(int nroFactura)
        {
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@nro_factura", SqlDbType.Int) { Value = nroFactura }
            };

            return DataHelper.GetInstance().ExecuteSPInsertUpdate("SP_ELIMINAR_FACTURA", parametros);
        }
    }
}

