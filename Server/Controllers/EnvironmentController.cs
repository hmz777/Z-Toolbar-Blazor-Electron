using BlazorElectronToolbar.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using Windows.UI.ViewManagement;

namespace BlazorElectronToolbar.Server.Controllers
{
    [ApiController]
    public class EnvironmentController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public EnvironmentController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [Route("GetWindowsAccentColor")]
        [HttpGet]
        public IActionResult GetWindowsAccentColor()
        {
            var uISettings = new UISettings();
            var color = uISettings.GetColorValue(UIColorType.Accent);

            return Ok(new { R = color.R, G = color.G, B = color.B, A = color.A });
        }

        [Route("GetAboutData")]
        [HttpGet]
        public IActionResult GetAboutData()
        {
            try
            {
                var data = configuration.GetSection("AppInfo:Developer").Get<AboutModel>();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
