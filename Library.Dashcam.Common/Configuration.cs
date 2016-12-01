using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Dashcam.Common
{
    public class Configuration
    {
        public int BitRate { get; set; }
        public int ClipLength { get; set; }
        public int Codec { get; set; }
        public int Device { get; set; }
        public string FileFormat { get; set; }
        public string FileNameFormat { get; set; }
        public int Height { get; set; }
        public bool ShowTimestamp { get; set; }
        public int Width { get; set; }
    }
}
