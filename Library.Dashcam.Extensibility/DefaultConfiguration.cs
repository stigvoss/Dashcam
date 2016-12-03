using System;
using Library.Dashcam.Extensibility.Interfaces;

namespace Library.Dashcam.Extensibility
{
    internal class DefaultConfiguration : IConfiguration
    {
        private bool _enable = true;

        public bool Enable
        {
            get
            {
                return _enable;
            }

            set
            {
                _enable = value;
            }
        }
    }
}