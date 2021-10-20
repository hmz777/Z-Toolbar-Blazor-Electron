using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorElectronToolbar.Shared
{
    public class AccentColor
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public override string ToString()
        {
            return $"rgba({R},{G},{B},{A})";
        }
    }
}
