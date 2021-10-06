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
        public Task<ScreenSize> GetScaledScreenSize();
    }
}
