using BlazorElectronToolbar.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorElectronToolbar.Client.Helpers
{
    public interface IToolbarCommandControl
    {
        public Task Expand();
        public Task Retract();
        public Task<ScreenSize> GetScaledScreenBounds();
        public Task OpenDevTools();
        public Task<bool> CreateFileIcon(string ItemId, string Path);
        public Task<bool> Run(string Path);
        public Task ShowInExplorer(string Path);
        public Task Remove(string FileId);
        public Task<AccentColor> GetWindowsAccentColor();
        public Task SaveChanges(IEnumerable<FileDescriptor> Files);
        public Task<IEnumerable<FileDescriptor>> LoadFiles();
        /// <summary>
        /// Returns whether the path points to a file or directory.
        /// </summary>
        /// <param name="Path"></param>
        /// <returns>True if directory, False if File</returns>
        public Task<bool> DirectoryOrFile(string Path);
        public Task<AboutModel> AboutDialog();
        public Task ExitApp();
    }
}
