using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Org.Proj.Image.BusinessLibrary.Contract;
using Org.Proj.Image.Common.Model;
using Org.Proj.Image.DataAccess.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Org.Proj.Image.BusinessLibrary
{
    public class FileManager : IFileManager
    {
        private static ImageProcdbContext _dbContext;

        public FileManager()
        {
            var connString = Environment.GetEnvironmentVariable("DbConnectionString");
            var services = new ServiceCollection();

            services.AddDbContext<ImageProcdbContext>
                (options => options.UseSqlServer(connString));

            var serviceProvider = services.BuildServiceProvider();

            _dbContext = serviceProvider.GetService<ImageProcdbContext>();
        }

        public FileManager(string connString)
        {
            var services = new ServiceCollection();

            services.AddDbContext<ImageProcdbContext>
                (options => options.UseSqlServer(connString));

            var serviceProvider = services.BuildServiceProvider();

            _dbContext = serviceProvider.GetService<ImageProcdbContext>();
        }

        public int UploadFileInfo(ImageFileInfo imgFile)
        {
            CsxFileInfo dbFile = new CsxFileInfo
            {
                FileName = imgFile.FileName,
                FilePath = imgFile.FilePath,
                FileStatusCode = 1,
                CreatedOn = imgFile.CreatedOn,
                UpdatedOn = imgFile.UpdatedOn
            };

            _dbContext.Add(dbFile);
            _dbContext.SaveChanges();

            return dbFile.Id;

        }

        public void UploadFileRecordInfo(ImageFileInfo imgFile)
        {
            IList<FileRecordInfo> recList = new List<FileRecordInfo>();

            foreach (var rec in imgFile.ImageFileDetail)
            {
                recList.Add(new FileRecordInfo
                {
                    ImagePath1 = rec.ImagePath1,
                    ImagePath2 = rec.ImagePath2,
                    CreatedOn = rec.CreatedOn,
                    UpdatedOn = rec.UpdatedOn,
                    FileId = imgFile.FileInfoId,
                    RecordStatusCode = 1,
                    ElapsedTime = null,
                    Score = null
                }); ;
            }

            _dbContext.AddRange(recList);
            _dbContext.SaveChanges();
        }

        public IList<ImageFileInfo> GetFileProcessingInfo()
        {
            var result = (from data in _dbContext.CsxFileInfo
                          select new ImageFileInfo
                          {
                              CreatedOn = data.CreatedOn,
                              FileInfoId = data.Id,
                              FileName = data.FileName,
                              FilePath = data.FilePath,
                              UpdatedOn = data.UpdatedOn,
                              FileStatusCode = Convert.ToInt32(data.FileStatusCode),
                              FileStatusDescription = (
                                                    Convert.ToInt32(data.FileStatusCode) == 1 ? "File Uploaded" :
                                                    Convert.ToInt32(data.FileStatusCode) == 2 ? "In Progress" :
                                                    Convert.ToInt32(data.FileStatusCode) == 3 ? "Processing Complete" :
                                                    Convert.ToInt32(data.FileStatusCode) == 4 ? "Processing Failed" : "Unknown"
                                                       )
                          }
                          ).OrderByDescending(x => x.FileInfoId).ToList();

            return result;
        }

    }
}
