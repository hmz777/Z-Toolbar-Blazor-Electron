using BlazorElectronToolbar.Server.Helpers;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorElectronToolbar.Server.Controllers
{
    [ApiController]
    public class WindowController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public WindowController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [Route("ExpandWindow")]
        [HttpGet]
        public async Task<IActionResult> ExpandWindowAction()
        {
            try
            {
                if (!StateHelpers.ToolbarExpanded)
                {
                    var WindowWidth = configuration.GetValue<int>("Window:Width");
                    var WindowHeight = configuration.GetValue<int>("Window:Height");
                    var ExpandAmount = configuration.GetValue<int>("Window:ExpandAmount");

                    var window = Electron.WindowManager.BrowserWindows.First();
                    var ScreenBounds = (await Electron.Screen.GetPrimaryDisplayAsync()).Bounds;

                    window.SetBounds(new Rectangle
                    {
                        X = ScreenBounds.Width - (WindowWidth + ExpandAmount),
                        Y = (ScreenBounds.Height - WindowHeight) / 2,
                        Width = WindowWidth + ExpandAmount,
                        Height = WindowHeight
                    }, true);

                    StateHelpers.ToolbarExpanded = true;
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("UnExpandWindow")]
        [HttpGet]
        public async Task<IActionResult> UnExpandWindow()
        {
            try
            {
                if (StateHelpers.ToolbarExpanded)
                {
                    var WindowWidth = configuration.GetValue<int>("Window:Width");
                    var WindowHeight = configuration.GetValue<int>("Window:Height");

                    var window = Electron.WindowManager.BrowserWindows.First();
                    var ScreenBounds = (await Electron.Screen.GetPrimaryDisplayAsync()).Bounds;

                    window.SetBounds(new Rectangle
                    {
                        X = ScreenBounds.Width - WindowWidth,
                        Y = (ScreenBounds.Height - WindowHeight) / 2,
                        Width = WindowWidth,
                        Height = WindowHeight
                    }, true);

                    StateHelpers.ToolbarExpanded = false;
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("GetScaledScreenBounds")]
        [HttpGet]
        public async Task<IActionResult> GetScaledScreenBounds()
        {
            //needs edits to bounds
            var ScreenBounds = (await Electron.Screen.GetPrimaryDisplayAsync()).Bounds;

            return Ok(new { Width = ScreenBounds.Width, Height = ScreenBounds.Height });
        }

        [Route("OpenDevTools")]
        [HttpGet]
        public IActionResult OpenDevTools()
        {
            var window = Electron.WindowManager.BrowserWindows.First();
            window.WebContents.OpenDevTools();

            return Ok();
        }

        [Route("ExitApp")]
        [HttpGet]
        public IActionResult ExitApp()
        {
            try
            {
                var MainWindow = Electron.WindowManager.BrowserWindows.First();

                if (MainWindow != null)
                {
                    MainWindow.Close();
                }

                return Ok();
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

    }
}
