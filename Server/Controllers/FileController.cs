using BlazorElectronToolbar.Server.Helpers;
using BlazorElectronToolbar.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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
                string iconPath;

                if (!webHostEnvironment.IsDevelopment())
                {
                    iconPath = System.IO.Path.Combine(webHostEnvironment.WebRootPath, "img", fileModel.FileId + ".png");
                }
                else
                {
                    iconPath = System.IO.Path.Combine(webHostEnvironment.ContentRootPath, "..", "Client", "wwwroot", "img", fileModel.FileId + ".png");
                }

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
                return StatusCode(500, ex.Message + " / " + "Content Root Path: " + webHostEnvironment.ContentRootPath + " Web Root: " + webHostEnvironment.WebRootPath);
            }
        }

        [Route("RemoveIcon")]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult RemoveIcon([FromBody] string FileId)
        {
            try
            {
                string iconPath;

                if (!webHostEnvironment.IsDevelopment())
                {
                    iconPath = System.IO.Path.Combine(webHostEnvironment.WebRootPath, "img", FileId + ".png");
                }
                else
                {
                    iconPath = System.IO.Path.Combine(webHostEnvironment.ContentRootPath, "..", "Client", "wwwroot", "img", FileId + ".png");
                }

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
                if (!Directory.Exists(Path) && !System.IO.File.Exists(Path))
                {
                    return Ok("File does not exist!");
                }

                var PP = new Process { StartInfo = new ProcessStartInfo(Path) { UseShellExecute = true } };
                var res = PP.Start();
                PP.Dispose();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("ShowInExplorer")]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult ShowInExplorer([FromBody] string Path)
        {
            try
            {
                if (!Directory.Exists(Path) && !System.IO.File.Exists(Path))
                {
                    return Ok("File does not exist!");
                }

                string Args = string.Format("/select,\"{0}\"", Path);

                var PP = new Process { StartInfo = new ProcessStartInfo("explorer.exe", Args) { UseShellExecute = true } };
                var res = PP.Start();
                PP.Dispose();

                return Ok();
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

                string path;

                if (!webHostEnvironment.IsDevelopment())
                {
                    path = System.IO.Path.Combine(webHostEnvironment.WebRootPath, savePath);
                }
                else
                {
                    path = System.IO.Path.Combine(webHostEnvironment.ContentRootPath, "..", "Client", "wwwroot", savePath);
                }

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

                string path;

                if (!webHostEnvironment.IsDevelopment())
                {
                    path = System.IO.Path.Combine(webHostEnvironment.WebRootPath, loadPath);
                }
                else
                {
                    path = System.IO.Path.Combine(webHostEnvironment.ContentRootPath, "..", "Client", "wwwroot", loadPath);
                }

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
