using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Dashcam.Common;
using System.Drawing;

namespace Flip
{
    public class Module : Library.Dashcam.Extensibility.Module
    {
        private const string MODULE_NAME = "Flip";

        public override string Name
        {
            get
            {
                return MODULE_NAME;
            }
        }

        public override void Execute(FrameInfo frameInfo)
        {
            Bitmap bitmap = frameInfo.Frame;

            bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
        }
    }
}
