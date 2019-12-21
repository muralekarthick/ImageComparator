using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using ProcessingEngine.Library;
using System.Diagnostics;
using System;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace ImageProcessingEngine
{
    public static class ImageProcessor
    {
        [FunctionName("ProcessImage")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            //Creating AppInsights Object for plugging in Telemetry
            TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();
            configuration.InstrumentationKey = "e64b3c27-ecbc-4960-bfd0-586c9326db2d";
            var telemetryClient = new TelemetryClient(configuration);

            log.Info("C# HTTP trigger function processed a request.");

            dynamic data = await req.Content.ReadAsAsync<object>();

            string id = data.Id;
            string file1 = data.ImagePath1;
            string file2 = data.ImagePath2;
            Guid transID = Guid.NewGuid();

            telemetryClient.TrackTrace("Started Process for id: " + id + " and the transaction id is " + transID.ToString(), SeverityLevel.Information);

            //Stop Watch to calculate the elapsed time - time taken for the Image comparison. 
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                Image image1 = new Bitmap(ImagePrep.GetStreamImage(file1));
                Image image2 = new Bitmap(ImagePrep.GetStreamImage(file2));

                //Image2 dimension is compared with Image1 and resized accordingly for the accuracy of SSIM algorithm.
                var width = image1.Width;
                var height = image1.Height;

                var modifiedImage = ImagePrep.ResizeImage(image2, width, height);

                stopWatch.Stop();
                decimal ts = (decimal)stopWatch.ElapsedMilliseconds / 1000;

                double result = SSIM.Compute(image1, modifiedImage);

                ResponseData resp = new ResponseData()
                {
                    ElapsedTime = ts,
                    Score = result,
                    Id = Convert.ToInt32(id),
                    TransactionId = transID
                };
                //Elapsed Time, Score, Image ID is passed back to the Orchestrator.
                return req.CreateResponse(HttpStatusCode.OK, resp);
            }
            catch(Exception e)
            {
                telemetryClient.TrackException(e);
                return req.CreateResponse(HttpStatusCode.ExpectationFailed, e.Message + " TRID:"+transID);
            }
        }
    }
}
