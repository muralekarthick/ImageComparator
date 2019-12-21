using Dapper;
using ImageProcessHelper.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessHelper
{
    public class ImageComparator
    {
        public static bool CompareImage(List<ImageDetail> imageurl)
        {
            bool isSuccessful = true;
            foreach (ImageDetail imageobj in imageurl)
            {
                using (var httpClient = new HttpClient())
                { 
                   StringContent content = new StringContent(JsonConvert.SerializeObject(imageobj), Encoding.UTF8, "application/json");
                    SqlHelper util = new SqlHelper();
                    try
                    {
                        using (var response = httpClient.PostAsync(Environment.GetEnvironmentVariable("ImageProcessEngineAPI"), content).Result)
                        //using (var response = httpClient.PostAsync("http://localhost:7071/api/ProcessImage", content).Result)
                        {
                            string apiResponse = response.Content.ReadAsStringAsync().Result;
                            var dbRec = JsonConvert.DeserializeObject<ResponseData>(apiResponse);
                            
                            util.updateData("ImgProc.UpdateData", GenerateQueryParameters(dbRec)).GetAwaiter().GetResult();
                        }
                    }
                    catch(Exception)
                    {
                        isSuccessful = false;
                        util.updateData("ImgProc.UpdateData", GenerateQueryParameters(Convert.ToInt32(imageobj.Id))).GetAwaiter().GetResult();
                    }
                }
            }
            return isSuccessful;
        }

        private static DynamicParameters GenerateQueryParameters(ResponseData param)
        {
            DynamicParameters queryParam = new DynamicParameters();
            queryParam.Add("@id", param.Id);
            queryParam.Add("@score", param.Score);
            queryParam.Add("@elapsedTime", param.ElapsedTime);
            queryParam.Add("@status", 3);
            queryParam.Add("@transactionid", param.TransactionId);
            return queryParam;
        }

        private static DynamicParameters GenerateQueryParameters(int id)
        {
            DynamicParameters queryParam = new DynamicParameters();
            queryParam.Add("@id", id);
            queryParam.Add("@score", null);
            queryParam.Add("@elapsedTime", null);
            queryParam.Add("@status", 4);
            queryParam.Add("@transactionid", null);
            return queryParam;
        }
    }
}
