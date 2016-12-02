using Library.Dashcam.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timestamp
{
    public class Module : Library.Dashcam.Extensibility.Module
    {
        private const string MODULE_NAME = "Timestamp";
        
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

            Bitmap frame = frameInfo.Frame;
            DateTime time = frameInfo.Time;
            string timestamp = time.ToString(configuration.Format);

            using (Graphics graphics = Graphics.FromImage(frame))
            {
                graphics.DrawString(timestamp, configuration.Font, configuration.Color, configuration.Position);
            }
        }
    }
}
