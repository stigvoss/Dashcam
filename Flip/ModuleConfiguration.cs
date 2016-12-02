﻿using Library.Dashcam.Extensibility.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flip
{
    public class ModuleConfiguration : IConfiguration
    {
        private int _degrees = 180;

        private string _flip = null;

        public int Degrees { get { return _degrees; } set { _degrees = value; } }

        public string Flip { get { return _flip; } set { _flip = value; } }
    }
}