using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorElectronToolbar.Shared
{
    public class AboutModel
    {
        public string Description { get; set; }
        public Dictionary<string, string> Links { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}
