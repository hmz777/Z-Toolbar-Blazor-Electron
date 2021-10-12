using BlazorElectronToolbar.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

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
