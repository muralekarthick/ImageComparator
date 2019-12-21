using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using ImageProcessHelper.Models;

namespace ImageProcessHelper
{
    public class CSVApiHelper
    {
        public static bool GenerateCSV(CSVApiRequest file)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(file), Encoding.UTF8, "application/json");
                SqlHelper util = new SqlHelper();
                try
                {
                    var queryParam = string.Format("fileId={0}&fileName={1}", file.Id, file.FileName);
                    using (var response = httpClient.PostAsync(Environment.GetEnvironmentVariable("CSVDownloadAPI") + queryParam, content).Result)
                    {
                        string apiResponse = response.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception)
                {
                    //TODO: Handle Exception.
                }
            }
            return true;
        }
    }
}
