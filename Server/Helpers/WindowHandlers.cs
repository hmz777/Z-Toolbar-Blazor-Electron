using ElectronNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorElectronToolbar.Server.Helpers
{
    public static class WindowHandlers
    {
        public async static void OnFocus()
        {
            var window = Electron.WindowManager.BrowserWindows.First();
            var pos = await window.GetPositionAsync();
            var size = await window.GetSizeAsync();
            window.SetPosition(pos[0] - size[0], pos[1]);
        }
    }
}
