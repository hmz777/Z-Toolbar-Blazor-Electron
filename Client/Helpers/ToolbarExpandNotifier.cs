using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorElectronToolbar.Client.Helpers
{
    public class ToolbarExpandNotifier
    {
        public event Action<bool> OnExpandToggle;

        public void Toggle(bool expand)
        {
            OnExpandToggle?.Invoke(expand);
        }
    }
}
