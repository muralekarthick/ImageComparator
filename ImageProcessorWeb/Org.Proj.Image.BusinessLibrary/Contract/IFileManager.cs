using Org.Proj.Image.Common.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Proj.Image.BusinessLibrary.Contract
{
    public interface IFileManager
    {
         int UploadFileInfo(ImageFileInfo imgFile);

         void UploadFileRecordInfo(ImageFileInfo imgFile);

        IList<ImageFileInfo> GetFileProcessingInfo();
        
    }
}
