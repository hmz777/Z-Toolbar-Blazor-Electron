﻿using BlazorElectronToolbar.Server.Helpers;
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

                    var ScreenSize = (await Electron.Screen.GetPrimaryDisplayAsync()).Bounds;

                    var window = Electron.WindowManager.BrowserWindows.First();
                    window.SetBounds(new Rectangle
                    {
                        X = ScreenSize.Width - (WindowWidth + ExpandAmount),
                        Y = (ScreenSize.Height - WindowHeight) / 2,
                        Height = WindowHeight,
                        Width = WindowWidth + ExpandAmount
                    });

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
                    var ScreenSize = (await Electron.Screen.GetPrimaryDisplayAsync()).Bounds;

                    var window = Electron.WindowManager.BrowserWindows.First();
                    window.SetBounds(new Rectangle
                    {
                        X = ScreenSize.Width - WindowWidth,
                        Y = (ScreenSize.Height - WindowHeight) / 2,
                        Height = WindowHeight,
                        Width = WindowWidth
                    });

                    StateHelpers.ToolbarExpanded = false;
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("GetScaledScreenSize")]
        [HttpGet]
        public async Task<IActionResult> GetScaledScreenSize()
        {
            var ScreenSize = (await Electron.Screen.GetPrimaryDisplayAsync()).Bounds;

            return Ok(new { Width = ScreenSize.X, Height = ScreenSize.Y });
        }

        [Route("OpenDevTools")]
        [HttpGet]
        public IActionResult OpenDevTools()
        {
            var window = Electron.WindowManager.BrowserWindows.First();
            window.WebContents.OpenDevTools();

            return Ok();
        }
    }
}
