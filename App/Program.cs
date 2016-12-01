using Library.Dashcam.Common;
using Library.Dashcam.Core;
using Library.Dashcam.Extensibility;
using Library.Dashcam.Extensibility.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace App
{
    class Program
    {
        private static string _device;
        private static IEnumerable<IModule> _modules;

        private Core _core;

        static void Main(string[] args)
        {
            Program program = new Program();

            program.Start();

            Console.WriteLine($"Device: {_device}");

            foreach (IModule module in _modules)
            {
                Console.WriteLine($"Loaded Module: {module.Name}");
            }

            Console.WriteLine();
            Console.Write("Press [ENTER] to exit...");
            Console.ReadLine();

            program.Stop();
        }

        public void Start()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            
            string root = Path.GetPathRoot(assembly.Location);

            string config = Path.Combine(root, Constants.FILE_NAME_CONFIGURATION);
            string bin = Path.Combine(root, Constants.DIRECTORY_NAME_BINARIES);
            string data = Path.Combine(root, Constants.DIRECTORY_NAME_DATA);
            string modules = Path.Combine(root, Constants.DIRECTORY_NAME_MODULES);

            if (File.Exists(config) &&
                Directory.Exists(bin) &&
                Directory.Exists(data) &&
                Directory.Exists(modules))
            {
                string content = File.ReadAllText(config);
                Configuration configuration = JsonConvert.DeserializeObject<Configuration>(content);

                _core = new Core(configuration, data);

                _device = _core.Device;
                _modules = _core.LoadModules(modules);
                
                _core.Start();
            }
        }

        public void Stop()
        {
            _core.Stop();
        }
    }
}
