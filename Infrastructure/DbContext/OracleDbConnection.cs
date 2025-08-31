using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DbContext
{
    public class OracleDbConnection : IDisposable
    {
        private readonly OracleConnection _connection;
        OracleConnection conn = default(OracleConnection);
        OracleDataReader reader = default(OracleDataReader);
        DataSet dataset = default(DataSet);
        DataTable datatable = default(DataTable);
        OracleCommand command = default(OracleCommand);
        public OracleDbConnection(string connectionString)
        {
            _connection = new OracleConnection(connectionString);
        }
        public void CreateSqlCommand(string SpName, OracleParameter[] parms)
        {
            command = new OracleCommand(SpName, _connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 1200;
            command.InitialLOBFetchSize = -1;
            if ((parms != null))
            {
                foreach (OracleParameter parameter in parms)
                {
                    if (parameter.Direction == ParameterDirection.Output && parameter.OracleDbType != OracleDbType.Varchar2)
                        command.Parameters.Add(parameter.ParameterName, parameter.OracleDbType, 4000).Direction = parameter.Direction;
                    else
                        command.Parameters.Add(parameter);
                }
            }

        }
        public string GetStringData(string SpName, OracleParameter[] parms)
        {
            CreateSqlCommand(SpName, parms);
            string jsonData = string.Empty;
            try
            {
                using (OracleDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Get JSON data
                        jsonData = reader.GetString(0);
                        return jsonData;
                    }
                    else
                    {
                        return null; // No data returned
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                command.Dispose();
                CleanDatabase();
            }

            return jsonData;
        }

        private object GetValue(string proname, OracleDataReader dreader)
        {
            try
            {
                object p = dreader[proname];
                if ((object.ReferenceEquals(p.GetType(), typeof(DBNull))))
                {
                    return null;
                }
                return p;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public List<T> GetList<T>(string SpName, OracleParameter[] parms)
        {
            CreateSqlCommand(SpName, parms);

            List<T> list = new List<T>();

            try
            {
                //command.InitialLOBFetchSize = -1;
                using (OracleDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        T local = (T)Activator.CreateInstance(typeof(T));
                        PropertyInfo[] properties = local.GetType().GetProperties();
                        foreach (PropertyInfo info in properties)
                        {
                            info.SetValue(local, this.GetValue(info.Name, reader), null);
                        }
                        list.Add(local);
                    }
                    return list;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                command.Dispose();
                CleanDatabase();
            }

            return list;
        }

        public IDbConnection GetConnection()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            return _connection;
        }

        public void Dispose()
        {
            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
            }

            _connection.Dispose();
        }

        public OracleParameter MakeOutParameter(OracleDbType ParamType, ParameterDirection ParmDirection)
        {

            return MakeParam("", ParmDirection, null, ParamType, 0, OracleCollectionType.None);
        }

        public OracleParameter MakeInParameter(object Value, OracleDbType ParamType)
        {
            return MakeParam("", ParameterDirection.Input, Value, ParamType, 0, OracleCollectionType.None);
        }
        public OracleParameter MakeCollectionParameter(object Value, OracleDbType ParamType, int ParamSize)
        {
            return MakeParam("", ParameterDirection.Input, Value, ParamType, ParamSize, OracleCollectionType.PLSQLAssociativeArray);
        }
        public void CleanDatabase()
        {

            try
            {
                if ((conn != null))
                {
                    //conn.CloseAsync();
                    conn.DisposeAsync();
                    OracleConnection.ClearPool(conn);
                }

                if ((reader != null))
                {
                    reader.DisposeAsync();
                }

                if ((command != null))
                {
                    command.DisposeAsync();
                }

                if ((datatable != null))
                {
                    datatable.Dispose();
                }


            }
            catch (Exception ex)
            {
            }
        }

        public OracleParameter MakeParam(string ParmName, ParameterDirection Direction, object Value, OracleDbType ParamType, int ParamSize, OracleCollectionType ParamCollectionType)
        {
            OracleParameter Param = default(OracleParameter);
            try
            {
                if (Direction == ParameterDirection.Input)
                {
                    Param = new OracleParameter(ParmName, ParamType, ParamSize, Direction);
                    Param.Value = Value;
                }
                else if (Direction == ParameterDirection.Output)
                {
                    Param = new OracleParameter(ParmName, ParamType, ParamSize, Direction);
                    Param.Value = Value;
                }
                else if (Direction == ParameterDirection.ReturnValue)
                {
                    Param = new OracleParameter(ParmName, ParamType, ParamSize, null, Direction);
                    Param.Value = Value;
                }

                if (ParamCollectionType == OracleCollectionType.PLSQLAssociativeArray)
                {
                    Param.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                }

            }
            catch (Exception ex)
            {
                throw ex;

            }
            return Param;

        }

        public (int status, string response) RunProcedureWithReturnValAndStatus(string SpName, OracleParameter[] parms)
        {

            CreateSqlCommand(SpName, parms);

            Int32 status = 0;
            string response = "0";

            try
            {
                command.ExecuteNonQuery();
                status = ((OracleDecimal)command.Parameters[0].Value).ToInt32();
                response = Convert.ToString((OracleString)command.Parameters[1].Value).Trim();

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                command.Dispose();
                CleanDatabase();

            }

            return (status, response);

        }

        public T GetModelData<T>(string SpName, OracleParameter[] parms)
        {
            CreateSqlCommand(SpName, parms);

            T local = (T)Activator.CreateInstance(typeof(T));

            try
            {
                using (OracleDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PropertyInfo[] properties = local.GetType().GetProperties();
                        foreach (PropertyInfo info in properties)
                        {
                            info.SetValue(local, this.GetValue(info.Name, reader), null);
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                command.Dispose();
                CleanDatabase();
            }

            return local;
        }




    }
}
