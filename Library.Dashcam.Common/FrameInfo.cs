using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Dashcam.Common
{
    public struct FrameInfo : IDisposable
    {
        public DateTime Time { get; set; }
        public Bitmap Frame { get; set; }

        public void Dispose()
        {
            Frame.Dispose();
        }
    }
}
