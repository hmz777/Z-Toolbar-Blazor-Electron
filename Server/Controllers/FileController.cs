using BlazorElectronToolbar.Server.Helpers;
using BlazorElectronToolbar.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorElectronToolbar.Server.Controllers
{
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment;

        public FileController(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        [Route("CreateFileIcon")]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult CreateFileIcon([FromBody] FileModel fileModel)
        {
            try
            {
                var iconPath = System.IO.Path.Combine(webHostEnvironment.ContentRootPath, "..", "Client", "wwwroot", "img", fileModel.FileId + ".png");

                if (!System.IO.File.Exists(iconPath))
                {
                    var fileIcon = Icon.ExtractAssociatedIcon(fileModel.Path);

                    using (var fileStream = new FileStream(iconPath, FileMode.Create))
                    {
                        fileIcon.Save(fileStream);
                        fileStream.Flush();
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("RemoveIcon")]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult RemoveIcon([FromBody] string FileId)
        {
            try
            {
                var iconPath = System.IO.Path.Combine(webHostEnvironment.ContentRootPath, "..", "Client", "wwwroot", "img", FileId + ".png");

                if (System.IO.File.Exists(iconPath))
                {
                    System.IO.File.Delete(iconPath);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("LaunchFile")]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult LaunchFile([FromBody] string Path)
        {
            try
            {
                var PP = new Process { StartInfo = new ProcessStartInfo(Path) { UseShellExecute = true } };
                var res = PP.Start();
                PP.Dispose();

                if (res)
                {                   
                    return Ok(true);
                }

                return Ok(false);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
