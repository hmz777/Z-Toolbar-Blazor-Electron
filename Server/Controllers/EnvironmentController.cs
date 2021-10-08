using Microsoft.AspNetCore.Mvc;

namespace BlazorElectronToolbar.Server.Controllers
{
    [ApiController]
    public class EnvironmentController : ControllerBase
    {
        //[Route("GetWindowsAccentColor")]
        //[HttpGet]
        //public IActionResult GetWindowsAccentColor()
        //{
        //    try
        //    {
        //        UISettings uISettings = new UISettings();
        //        var color = uISettings.GetColorValue(UIColorType.Accent);

        //        return Ok(new { R = color.R, G = color.G, B = color.B, A = color.A });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}
    }
}
