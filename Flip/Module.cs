using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Dashcam.Common;
using System.Drawing;
using Toolbox.Extensions;

namespace Flip
{
    public class Module : Library.Dashcam.Extensibility.Module
    {
        private const string MODULE_NAME = "Flip and Rotate";

        public override Type ConfigurationType
        {
            get
            {
                return typeof(ModuleConfiguration);
            }
        }

        public override string Name
        {
            get
            {
                return MODULE_NAME;
            }
        }

        public override void Execute(FrameInfo frameInfo)
        {
            ModuleConfiguration configuration = (ModuleConfiguration)Configuration;

            Bitmap bitmap = frameInfo.Frame;

            RotateFlipType action;

            switch (configuration.Degrees)
            {
                default:
                case 0:
                    action = RotateFlipType.RotateNoneFlipNone;
                    break;
                case 90:
                    action = RotateFlipType.Rotate90FlipNone;
                    break;
                case 180:
                    action = RotateFlipType.Rotate180FlipNone;
                    break;
                case 270:
                    action = RotateFlipType.Rotate270FlipNone;
                    break;

            }

            bitmap.RotateFlip(action);

            switch (configuration.Flip)
            {
                default:
                case null:
                    action = RotateFlipType.RotateNoneFlipNone;
                    break;
                case "X":
                    action = RotateFlipType.RotateNoneFlipX;
                    break;
                case "Y":
                    action = RotateFlipType.RotateNoneFlipY;
                    break;
                case "YX":
                case "XY":
                    action = RotateFlipType.RotateNoneFlipXY;
                    break;

            }

            bitmap.RotateFlip(action);
        }
    }
}
