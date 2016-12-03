using Library.Dashcam.Extensibility.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Dashcam.Common;

namespace Library.Dashcam.Extensibility
{
    public abstract class Module : IModule
    {
        private int _weight = 1000;
        private bool _enable = true;

        private IConfiguration _configuration = default(IConfiguration);

        public abstract ModuleCapabilities[] Capabilities { get; }

        public IConfiguration Configuration
        {
            get
            {
                return _configuration;
            }
            set
            {
                _configuration = value;
            }
        }

        public int Weight
        {
            get
            {
                return _weight;
            }
            set
            {
                _weight = value;
            }
        }

        public abstract string Name { get; }

        public abstract Type ConfigurationType { get; }

        public abstract void Execute(FrameInfo frameInfo);
    }
}
