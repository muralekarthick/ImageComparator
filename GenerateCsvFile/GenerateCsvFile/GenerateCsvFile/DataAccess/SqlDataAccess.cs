using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace GenerateCsvFile.DataAccess
{
    public class SqlDataAccess
    {
        string _dbConnectionString;
        public SqlDataAccess(string dbConnectionString)
        {
            _dbConnectionString = dbConnectionString;
        }
        public DataTable ExecuteReader(string procName, SqlParameter[] sqlParams = null)
        {
            DataTable dataTable = null;
            SqlConnectionStringBuilder sqlConnectionString = new SqlConnectionStringBuilder(_dbConnectionString);
            try
            {

                using (var sqlConnection = new SqlConnection(sqlConnectionString.ConnectionString))
                {
                    sqlConnection.Open();
                    using (var sqlCmd = new SqlCommand())
                    {
                        sqlCmd.Connection = sqlConnection;
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.CommandText = procName;
                        if (sqlParams != null)
                        {
                            foreach (var parameter in sqlParams)
                                sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = parameter.ParameterName, Value = parameter.Value, SqlDbType = parameter.SqlDbType });
                        }
                        using (var reader = sqlCmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            dataTable = new DataTable();
                            dataTable.Load(reader);
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                //TODO: Add telemetry
                throw ex;
            }
            catch (Exception ex)
            {
                //TODO: Add telemetry
                throw ex;
            }

            return dataTable;
        }

        public int ExecuteNonQuery(string procName, SqlParameter[] sqlParams)
        {
            int result = 0;
            SqlConnectionStringBuilder sqlConnectionString = new SqlConnectionStringBuilder(_dbConnectionString);

            try
            {
                using (var sqlConnection = new SqlConnection(sqlConnectionString.ConnectionString))
                {
                    sqlConnection.Open();
                    using (var sqlCmd = new SqlCommand())
                    {

                        sqlCmd.Connection = sqlConnection;
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.CommandText = procName;

                        if (sqlParams != null)
                        {
                            foreach (var parameter in sqlParams)
                            {
                                sqlCmd.Parameters.Add(new SqlParameter() { ParameterName = parameter.ParameterName, Value = parameter.Value, SqlDbType = parameter.SqlDbType });
                            }
                        }
                        result = sqlCmd.ExecuteNonQuery();
                    }
                }

            }
            catch (SqlException exp)
            {
                throw;
            }
            catch (Exception e)
            {
                throw;
            }
            return result;
        }
    }
}
