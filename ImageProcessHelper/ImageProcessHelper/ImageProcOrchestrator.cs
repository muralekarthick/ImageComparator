using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ImageProcessHelper.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Dapper;

namespace ImageProcessHelper
{
    public static class ImageProcOrchestrator
    {
        [FunctionName("ImageProcOrchestrator")]
        public static async Task Run([TimerTrigger(" 0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {

            try
            {
                var filetoprocessList = await SqlHelper.GetFiles<CSVApiRequest>();

                if (filetoprocessList.Count != 0)
                {
                    //Get file to process 
                    foreach (var file in filetoprocessList)
                    {

                        //Get records for file 
                        DynamicParameters queryParam = new DynamicParameters();
                        queryParam.Add("@id", file.Id);
                        var imagetoprocessList = await SqlHelper.GetData<ImageDetail>(queryParam);

                        //Process records
                        ImageComparator.CompareImage(imagetoprocessList);

                        //Generate Csv file
                        CSVApiHelper.GenerateCSV(file);

                    }

                }
            }
            catch (System.Exception)
            {
                //TODO: Handle exception
                throw;
            }
        }
    }
}
