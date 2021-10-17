using BlazorElectronToolbar.Server.Helpers;
using BlazorElectronToolbar.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorElectronToolbar.Server.Controllers
{
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IConfiguration configuration;

        public FileController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.configuration = configuration;
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

                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("SaveChanges")]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult SaveChanges(IEnumerable<FileDescriptor> Files)
        {
            try
            {
                var savePath = configuration.GetValue<string>("SavePath");
                var path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), savePath);

                var json = JsonSerializer.Serialize<IEnumerable<FileDescriptor>>(Files);
                System.IO.File.WriteAllText(path, json);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("LoadFiles")]
        [HttpGet]
        public IActionResult LoadFiles()
        {
            try
            {
                var loadPath = configuration.GetValue<string>("SavePath");
                var path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), loadPath);

                if (System.IO.File.Exists(path))
                {
                    var json = System.IO.File.ReadAllText(path);
                    var files = JsonSerializer.Deserialize<IEnumerable<FileDescriptor>>(json);
                    return Ok(files);
                }
                else
                {
                    return Ok(Enumerable.Empty<FileDescriptor>());
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("DirectoryOrFile")]
        [HttpGet]
        public IActionResult DirectoryOrFile(string Path)
        {
            return new JsonResult(System.IO.Directory.Exists(Path));
        }
    }
}
