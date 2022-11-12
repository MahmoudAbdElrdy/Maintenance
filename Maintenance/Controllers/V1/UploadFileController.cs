using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.UploadHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace Maintenance.Controllers.V1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class UploadFileController : ApiBaseController
    {
        private readonly IWebHostEnvironment _env;

        public UploadFileController(IWebHostEnvironment env)
        {
            _env = env;


        }
        [HttpPost("UploadFile")]
        public IActionResult UploadFile()
        {
            ResponseDTO res;
            try
            {
                var name = Upload.SaveFile(Request.Form.Files[0]);
                //string path = xx[0];
                res = new ResponseDTO()
                {
                    StatusEnum = StatusEnum.Success,
                    Message = "SavedSuccessfully",
                    Result = name,
                };
            }
            catch (Exception ex)
            {
                res = new ResponseDTO()
                {
                    StatusEnum = StatusEnum.Failed,
                    Message = "Failed",
                    Result = null,
                };
            }
            return Ok(res);
        }
        //[HttpPost("FileUpload")]
        //public async Task<IActionResult> index(List<IFormFile> files)
        //{
        //    if (files == null || files.Count == 0)
        //        return Content("file not selected");
        //    long size = files.Sum(f => f.Length);
        //    var filePaths = new List<string>();
        //    foreach (var formFile in files)
        //    {
        //        if (formFile.Length > 0)
        //        {
        //            // full path to file in temp location
        //            var folderName = Path.Combine("wwwroot/UploadFiles");
        //            if (!Directory.Exists(folderName))
        //            {
        //                Directory.CreateDirectory(folderName);
        //            }

        //            string extension = Path.GetExtension(formFile.FileName);


        //            string fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
        //            var fileNameWithDate = fileName + DateTime.Now.Ticks;
        //            var filepath = Path.Combine(folderName, fileNameWithDate + extension);


        //            using (var stream = new FileStream(filepath, FileMode.Create))
        //            {
        //                await formFile.CopyToAsync(stream);
        //            }
        //            filePaths.Add(fileNameWithDate + extension);
        //        }
        //    }
        //    // process uploaded files
        //    // Don't rely on or trust the FileName property without validation.
        //    return Ok(new { count = files.Count, size, filePaths });
        //}
        [HttpGet("Base")]
        public string ServerRootPath()
        {

            return $"{Request.Scheme}://{Request.Host}{Request.PathBase}" + "/wwwroot/UploadFiles/";
        }
    }
}
