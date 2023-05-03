using BlazorApp1.Auth;
using BlazorApp1.Models;
using BlazorApp1.Models.Mobile;
using BlazorApp1.Models.Mobile.Responses;
using BlazorApp1.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace BlazorApp1.Controllers
{
    //../api/Upload/Save
    //../api/Upload/Remove
    //http://62.113.105.29:5454/api/Upload/UploadFiles
    [Route("api/[controller]/[action]")]
    public class UploadController : Controller
    {
        public IWebHostEnvironment HostingEnvironment { get; set; }
        public UploadController(IWebHostEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> UploadFiles(IFormFile[] files)
        {
            try
            {
                if (files != null)
                {
                    List<TFile> response = new List<TFile>();
                    foreach (var file in files)
                    {
                        string oldFileName = file.FileName;
                        string newFileName = string.Concat(Guid.NewGuid().ToString(), Path.GetExtension(file.FileName));

                        //!!! для тестирования заливать сразу в UploadDir
                        string saveLocation = Path.Combine(UploadPath.UploadDir, newFileName);
                        //string saveLocation = Path.Combine(UploadPath.TempDir, newFileName);

                        using (var fileStream = new FileStream(saveLocation, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);

                            TFile tfile = new TFile();
                            FileInfo fi = new FileInfo(saveLocation);

                            tfile.Name = fi.Name;
                            tfile.Size = fi.Length;
                            tfile.InitialName = oldFileName;
                            tfile.FullPath = Path.Combine(Utils.UploadPath.UploadUrl, fi.Name);
                            response.Add(tfile);
                        }
                    }
                    return Ok(new BaseMobileResponse(response, (int)ErrorHandling.ErrorCodes.ERROR_SUCCESS));
                }

                return new EmptyResult();

            }
            catch(Exception ex)
            {
                return StatusCode(500, new BaseMobileResponse("[]", ex.Message));
            }

        }

        //[HttpPost]
        //public ActionResult Remove(string file)
        //{
        //    if (file != null)
        //    {
        //        try
        //        {
        //            var fileLocation = Path.Combine(UploadPath.TempDir, file);
        //            if (System.IO.File.Exists(fileLocation))
        //            {
        //                System.IO.File.Delete(fileLocation);
        //            }
        //        }
        //        catch
        //        {
        //            Response.StatusCode = 500;
        //            Response.WriteAsync("File deletion failed.");
        //        }
        //    }

        //    return new EmptyResult();
        //}
    }
}
