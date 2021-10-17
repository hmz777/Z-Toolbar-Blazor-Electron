using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorElectronToolbar.Client.Shared.ContextMenu
{
    public class CMEventArgs
    {
        public MouseEventArgs MouseEventArgs;
        public string ItemId;
        public bool Checked { get; set; }
    }
}
