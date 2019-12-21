using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.Proj.Image.BusinessLibrary;
using Org.Proj.Image.Common.Model;

namespace Org.Proj.Image.Function
{
    public static class UploadFile
    {
        [FunctionName("UploadFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            bool isSuccess = false;
            var errorMessage = string.Empty;

            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");


                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<ImageFileInfo>(requestBody);

                FileManager file = new FileManager();
                var result = file.UploadFileInfo(data);

                data.FileInfoId = result;

                file.UploadFileRecordInfo(data);

                isSuccess = true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return isSuccess == true
                ? (ActionResult)new OkObjectResult(string.Empty)
                : new BadRequestObjectResult(errorMessage);
        }
    }
}
