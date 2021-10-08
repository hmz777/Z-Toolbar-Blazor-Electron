using BlazorElectronToolbar.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorElectronToolbar.Client.Helpers
{
    public class ToolbarCommandControl : IToolbarCommandControl
    {
        private readonly HttpClient httpClient;
        private readonly ToolbarExpandNotifier toolbarExpandNotifier;
        private bool ToolbarWorking = false;

        public ToolbarCommandControl(HttpClient httpClient, ToolbarExpandNotifier toolbarExpandNotifier)
        {
            this.httpClient = httpClient;
            this.toolbarExpandNotifier = toolbarExpandNotifier;
        }

        public async Task Expand()
        {
            if (!ToolbarWorking)
            {
                ToolbarWorking = true;

                await httpClient.GetAsync("/ExpandWindow");

                await Task.Delay(150);

                toolbarExpandNotifier.Toggle(true);

                ToolbarWorking = false;
            }
        }

        public async Task Retract()
        {
            if (!ToolbarWorking)
            {
                ToolbarWorking = true;

                toolbarExpandNotifier.Toggle(false);

                await Task.Delay(1000);

                await httpClient.GetAsync("/UnExpandWindow");

                ToolbarWorking = false;
            }
        }

        public async Task<ScreenSize> GetScaledScreenSize()
        {
            var res = await httpClient.GetAsync("/GetScaledScreenSize");
            return await res.Content.ReadFromJsonAsync<ScreenSize>();
        }

        public async Task OpenDevTools()
        {
            await httpClient.GetAsync("/OpenDevTools");
        }

        public async Task<bool> CreateFileIcon(string ItemId, string Path)
        {
            var res = await httpClient.PostAsJsonAsync("/CreateFileIcon", new FileModel { FileId = ItemId, Path = Path });
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> Run(string Path)
        {
            var res = await httpClient.PostAsJsonAsync("/LaunchFile", Path);

            return await res.Content.ReadFromJsonAsync<bool>();
        }

        public async Task AboutDialog()
        {
            await httpClient.GetAsync("/About");
        }

        public async Task Remove(string FileId)
        {
            await httpClient.PostAsJsonAsync("/RemoveIcon", FileId);
        }

        public async Task<AccentColor> GetWindowsAccentColor()
        {
            var color = await httpClient.GetFromJsonAsync<AccentColor>("/GetWindowsAccentColor");

            return color;
        }
    }
}
