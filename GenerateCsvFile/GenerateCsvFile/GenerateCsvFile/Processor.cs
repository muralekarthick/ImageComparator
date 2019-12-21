using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using GenerateCsvFile.Excel;
using GenerateCsvFile.DataAccess;
using System.Data;
using System.Data.SqlClient;
using GenerateCsvFile.DataContract;
using System.Collections.Generic;

namespace GenerateCsvFile
{
    public static class Processor
    {
        [FunctionName("GenerateCsvFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //Initalizing connection strings
            var blobConnectionString = Environment.GetEnvironmentVariable("BlobConnectionString");
            var blobContainerName = Environment.GetEnvironmentVariable("BlobContainerName");

            //Initializing helper classes
            ExcelHelper excelHelper = new ExcelHelper();
            BlobAccess blobAccess = new BlobAccess(blobConnectionString);


            //Initialize required parameters
            bool result = true;
            string fileName = req.Query["fileName"];
            int? fileId = null;
            if (!string.IsNullOrWhiteSpace(req.Query["fileId"]))
                fileId = int.Parse(req.Query["fileId"]);            
            
            //Validating required parameters
            if (fileId.HasValue && fileName != null)
            {
                try
                {
                    //Get processed records with score and elapsed time
                    var lstFileRecord = GetFileRecords(fileId.Value);

                    //Create Excel stream
                    var excelStream = excelHelper.CreateExcel(lstFileRecord);

                    //Upload to excel file to blob   
                    string blobFileName = string.Format("{0}/{1}_{2}.csv", "CSVLZ", fileName, DateTime.UtcNow.Ticks.ToString());
                    var uploadFile = blobAccess.UploadBlob(excelStream, blobFileName, blobContainerName);
                    uploadFile.GetAwaiter().GetResult();
                    var blobFileAsUri = blobAccess.GetFileAsUri(false, 24, false, blobFileName, blobContainerName);

                    //Update file path to dB
                    UpdateFilePath(fileId.Value, blobFileAsUri);
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            else
                return new BadRequestObjectResult("Please pass fileId and fileName as query parameters");

            return (ActionResult)new OkObjectResult(result);
        }

        public static List<FileRecord> GetFileRecords(int fileId)
        {
            var dbConnectionString = Environment.GetEnvironmentVariable("DbConnectionString");
            SqlDataAccess sqlDataAccess = new SqlDataAccess(dbConnectionString);
            var lstFileRecord = new List<FileRecord>();
            var parameters = new SqlParameter[]
            {
                new SqlParameter() { ParameterName = "@fileId", Value = fileId, SqlDbType = SqlDbType.Int}
            };
            try
            {
                //Fetch records
                DataTable result = sqlDataAccess.ExecuteReader(Constants.sp_GetAllRecords, parameters);
                foreach (DataRow row in result.Rows)
                {
                    var record = new FileRecord();
                    record.Image1 = row["ImagePath1"].ToString();
                    record.Image2 = row["ImagePath2"].ToString();
                    var stausCode = int.Parse(row["RecordStatusCode"].ToString());
                    if (stausCode != 4)
                    {
                        record.ElapsedTime = row["ElapsedTime"].ToString();
                        record.Score = row["Score"].ToString();
                    }
                    else
                    {
                        record.ElapsedTime = "Processing Failed";
                        record.Score = "Processing Failed";
                    }

                    lstFileRecord.Add(record);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstFileRecord;
        }

        public static bool UpdateFilePath(int fileId, string filePath)
        {
            var dbConnectionString = Environment.GetEnvironmentVariable("DbConnectionString");
            SqlDataAccess sqlDataAccess = new SqlDataAccess(dbConnectionString);
            var lstFileRecord = new List<FileRecord>();
            var parameters = new SqlParameter[]
            {
                new SqlParameter() { ParameterName = "@fileId", Value = fileId, SqlDbType = SqlDbType.Int},
                new SqlParameter() { ParameterName = "@filePath", Value = filePath, SqlDbType = SqlDbType.NVarChar}
            };
            try
            {
                //Update filePath
                var result = sqlDataAccess.ExecuteNonQuery(Constants.sp_UpdateFilePath, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
    }
}
