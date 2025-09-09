using Microsoft.Azure.Amqp.Framing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica01.Datos
{
    public class DataHelper
    {
        private static DataHelper? _instance;
        private SqlConnection _conecction;
        private DataHelper()
        {
            _conecction = new SqlConnection(@"");
        }
        public static DataHelper GetInstance()
        {
            {
                if (_instance == null)
                {
                    _instance = new DataHelper();
                }
                return _instance;
            }
        }
        public DataTable ExecuteSPQuery(string sp)
        {
            DataTable dt = new DataTable();
            try
            {
                _conecction.Open();
                var cmd = new SqlCommand(sp, _conecction);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = sp;
                dt.Load(cmd.ExecuteReader());
                _conecction.Close();
            }
            catch (SqlException ex)
            {
                dt = null;
            }
            finally
            {
                _conecction.Close();
            }
            return dt;
        }

        public DataTable ExecuteSPNonQuery(string sp, List<SqlParameter> parameters)

        {
            DataTable dt = new DataTable();
            try
            {
                _conecction.Open();
                using (var cmd = new SqlCommand(sp, _conecction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters.ToArray());

                    using (var reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }
            catch (SqlException)
            {
                dt = null;
            }
            finally
            {
                if (_conecction.State == ConnectionState.Open)
                    _conecction.Close();
            }
            return dt;
        }

        public bool ExecuteSPInsertUpdate(string spName, List<SqlParameter> parametros)
        {
            bool ok = false;
            try
            {
                _conecction.Open();
                using (var cmd = new SqlCommand(spName, _conecction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parametros != null)
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                    }

                    ok = cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (SqlException ex)
            {
                // Podés loguear el error si querés ver detalles
                Console.WriteLine("Error en DB: " + ex.Message);
                ok = false;
            }
            finally
            {
                if (_conecction.State == ConnectionState.Open)
                    _conecction.Close();
            }

            return ok;
        }

        public SqlConnection GetConnection()
        {
            return _conecction;
        }

        //public int ExecuteSPDMLTransact(string sp, List<SqlParameter> parametros, SqlTransaction cnn)
        //{
        //    int rowsAffected = 0;
        //    try
        //    {
        //        _conecction.Open();
        //        using (var cmd = new SqlCommand(sp, _conecction, transaction))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            if (parametros != null)
        //            {
        //                foreach (var param in parametros)
        //                {
        //                    cmd.Parameters.AddWithValue(param.ParameterName, param.Value);
        //                }
        //            }
        //            rowsAffected = cmd.ExecuteNonQuery();
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        // Podés loguear el error si querés ver detalles
        //        Console.WriteLine("Error en DB: " + ex.Message);
        //        rowsAffected = -1; // Indica un error
        //    }
        //    return rowsAffected;
        //}
    }
}
