using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Newtonsoft.Json;
using Org.Proj.Image.BusinessLibrary;
using Org.Proj.Image.Processor.Models;

namespace Org.Proj.Image.Processor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;

        public HomeController(ILogger<HomeController> logger, IConfiguration iConfig, IHostingEnvironment env)
        {
            _logger = logger;
            _configuration = iConfig;
            _environment = env;
        }

        public IActionResult Index()
        {
            var summaryRecords = GetUploadSummaryRecords();

            ViewBag.FileUploadedURL = TempData["FileUploadMessage"] != null ? TempData["FileUploadMessage"] : string.Empty;

            ViewBag.ImageUploadedURL = TempData["ImageUploadMessage"] != null ? TempData["ImageUploadMessage"] : string.Empty;

            return View(summaryRecords);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ImageIO()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> FileUploadAsync(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var fileContent = reader.ReadToEnd();
                    var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                    var fileName = parsedContentDisposition.FileName;

                    var result = ValidateInputFile(file, fileContent);
                    if (!result)
                        return View("Index", GetUploadSummaryRecords());

                    List<ImageFileRecord> filerec = new List<ImageFileRecord>();

                    foreach (var row in fileContent.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {
                            var img1 = Regex.Replace(row.Split(',')[0], @"\r", "");
                            var img2 = Regex.Replace(row.Split(',')[1], @"\r", "");

                            filerec.Add(new ImageFileRecord
                            {
                                ImagePath1 = img1,
                                ImagePath2 = img2,
                                CreatedOn = DateTime.UtcNow,
                                UpdatedOn = DateTime.UtcNow
                            }); ;

                        }
                    }

                    filerec.RemoveAt(0);

                    ImageFile imgfile = new ImageFile
                    {
                        FileName = System.IO.Path.GetFileName(fileName).Replace("\"", ""),
                        FilePath = string.Empty,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        ImageFileDetail = filerec
                    };

                    var apiURL = _configuration.GetValue<string>("APIUrl");
                    apiURL += "api/UploadFile";

                    using (var httpClient = new HttpClient())
                    {
                        StringContent content = new StringContent(JsonConvert.SerializeObject(imgfile), Encoding.UTF8, "application/json");

                        using (var response = await httpClient.PostAsync(apiURL, content))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            var dbRec = JsonConvert.DeserializeObject<ImageFile>(apiResponse);

                            TempData["FileUploadMessage"] = "CSV file successfully uploaded. Results will be available shortly under summary section.";
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.UploadFileError = "Please select a valid csv file";
                return View("Index", GetUploadSummaryRecords());
            }
        }


        [HttpPost]
        public async Task<ActionResult> ImageUploadAsync(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                string fileMimeType = file.ContentType;

                if (!fileMimeType.Contains("image"))
                {
                    ViewBag.UploadImageError = "Please select only image files of type TIFF, JPEG, GIF, PNG";
                    return View("Index", GetUploadSummaryRecords());
                }

                var webRoot = _environment.WebRootPath;
                var serverFilePath = System.IO.Path.Combine(webRoot, file.FileName);

                using (var fileStream = new FileStream(serverFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var accountName = _configuration.GetValue<string>("BlobAccountName");
                var accountKey = _configuration.GetValue<string>("BlobAccountKey");
                var containerName = _configuration.GetValue<string>("BlobContainerName");

                var storageCredentials = new StorageCredentials(accountName, accountKey);
                var cloudStorageAccount = new CloudStorageAccount(storageCredentials, true);
                var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

                var container = cloudBlobClient.GetContainerReference(containerName);

                var fileName = Path.GetFileName(file.FileName);
                var folderName = "LandingZone/" + fileName;

                var newBlob = container.GetBlockBlobReference(folderName);
                newBlob.Properties.ContentType = fileMimeType;

                await newBlob.UploadFromFileAsync(serverFilePath);

                // Reset the md5 Property
                newBlob.Properties.ContentMD5 = string.Empty;
                await newBlob.SetPropertiesAsync();

                TempData["ImageUploadMessage"] = "Image file successfully uploaded to blob URL : " + newBlob.Uri.AbsoluteUri;
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.UploadImageError = "Please select an image file to upload";
                return View("Index", GetUploadSummaryRecords());
            }
        }

        public async Task<IActionResult> DownloadFile(string filePath)
        {
            if (filePath != string.Empty)
            {
                var downloadFileName = filePath.Split('/').Last();

                var accountName = _configuration.GetValue<string>("BlobAccountName");
                var accountKey = _configuration.GetValue<string>("BlobAccountKey");
                var containerName = _configuration.GetValue<string>("BlobContainerName");

                var storageCredentials = new StorageCredentials(accountName, accountKey);
                var cloudStorageAccount = new CloudStorageAccount(storageCredentials, true);
                var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

                var container = cloudBlobClient.GetContainerReference(containerName);

                var fileName = Path.GetFileName(filePath);
                var folderName = "CSVLZ/" + downloadFileName;

                var newBlob = container.GetBlockBlobReference(folderName);

                MemoryStream ms = new MemoryStream();

                await newBlob.DownloadToStreamAsync(ms);

                Stream blobStream = newBlob.OpenReadAsync().Result;
                return File(blobStream, newBlob.Properties.ContentType, downloadFileName);
            }
            else
            {
                ViewBag.DownloadFileError = "File Does not exists";
                return View("Index", GetUploadSummaryRecords());
            }
        }

        private IList<ImageFile> GetUploadSummaryRecords()
        {
            IList<ImageFile> imgFileList = new List<ImageFile>();

            var dbConnString = _configuration.GetValue<string>("DbConnectionString");

            FileManager file = new FileManager(dbConnString);
            var result = file.GetFileProcessingInfo();

            foreach (var data in result)
            {
                imgFileList.Add(new ImageFile
                {
                    CreatedOn = data.CreatedOn,
                    FileInfoId = data.FileInfoId,
                    FileName = data.FileName,
                    FilePath = data.FilePath,
                    UpdatedOn = data.UpdatedOn,
                    FileStatusCode = Convert.ToInt32(data.FileStatusCode),
                    FileStatusDescription = data.FileStatusDescription
                });
            }

            return imgFileList;
        }

        public bool ValidateInputFile(IFormFile file, string fileContent)
        {
            bool isValid = true;

            if (!file.FileName.Contains(".csv"))
            {
                ViewBag.UploadFileError = "Please select only csv file.";
                isValid = false;

                return isValid;
            }

            var header = fileContent.Split('\n')[0];

            if ((header.Trim().Replace(" ", "").ToLower()) != "image1,image2")
            {
                ViewBag.UploadFileError = "File column headers are not in expected format.";
                isValid = false;

                return isValid;
            }

            if (fileContent.Split('\n').Length <= 1)
            {
                ViewBag.UploadFileError = "File does not have any rows";
                isValid = false;
            }

            return isValid;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
