using Library.Dashcam.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Dashcam.Extensibility.Interfaces
{
    public interface IModule
    {
        string Name { get; }

        int Weight { get; set; }

        ModuleCapabilities[] Capabilities { get; }

        IConfiguration Configuration { get; set; }

        void Execute(FrameInfo frameInfo);
    }
}
