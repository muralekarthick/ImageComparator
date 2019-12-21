using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Contentful.Core.Models;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.Proj.Image.Processor.Models;

namespace Org.Proj.Image.Processor.Controllers
{
    public class ImageIOController : Controller
    {

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
                        FilePath = fileName.Substring(0, fileName.LastIndexOf("\\")).Replace("\"", ""),
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        ImageFileDetail = filerec
                    };


                    using (var httpClient = new HttpClient())
                    {
                        StringContent content = new StringContent(JsonConvert.SerializeObject(imgfile), Encoding.UTF8, "application/json");

                        using (var response = await httpClient.PostAsync("http://localhost:7071/api/UploadFile", content))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            var dbRec = JsonConvert.DeserializeObject<ImageFile>(apiResponse);
                        }
                    }

                }
            }
           

            return RedirectToAction("UploadDocument");
        }

        // GET: ImageIO/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ImageIO/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ImageIO/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ImageIO/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ImageIO/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ImageIO/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ImageIO/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}