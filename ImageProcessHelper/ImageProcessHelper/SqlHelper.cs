using System;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;

namespace ImageProcessHelper
{
    public class SqlHelper
    {
        private static string connectionstring = Environment.GetEnvironmentVariable("connectionstring");

        private static string SPName = "[Imgproc].[SPReadytoPick]";
        public static async Task<List<T>> GetData<T>(Object param)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionstring))
                {
                    con.Open();
                    var result = await con.QueryAsync<T>(SPName, param, null, null, CommandType.StoredProcedure);
                    return result.ToList<T>();
                }
            }
            catch (Exception)
            {
                //TODO: Handle exception
                throw;
            }
        }

        public static async Task<List<T>> GetFiles<T>()
        {
            string SPNamefile = "[Imgproc].SPReadytoPickFile";
            try
            {
                using (SqlConnection con = new SqlConnection(connectionstring))
                {
                    con.Open();
                    var result = await con.QueryAsync<T>(SPNamefile, CommandType.StoredProcedure);
                    return result.ToList<T>();
                }
            }
            catch (Exception)
            {
                //TODO: Handle exception
                throw;
            }
        }

        public async Task updateData(string spname, Object param)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionstring))
                {
                    con.Open();
                    var result = await con.QueryAsync(spname, param, null, null, CommandType.StoredProcedure);
                    //return result.ToList<T>();
                }
            }
            catch (Exception)
            {
                //TODO: Handle exception
                throw;
            }
        }
    }
}