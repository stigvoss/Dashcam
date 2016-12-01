using Library.Dashcam.Extensibility.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Library.Dashcam.Extensibility
{
    public static class ModuleLoader
    {
        private const string MODULE_FILENAME_PATTERN = "*.dll";

        private static readonly List<IModule> _modules = new List<IModule>();

        public static List<IModule> Modules { get { return _modules; } }

        public static void Load(string location)
        {
            Uri uri = new Uri(location);
            Load(uri);
        }

        public static void Load(Uri location)
        {
            List<IModule> modules = new List<IModule>();

            string[] fileNames = GetModuleFileNames(location);

            foreach (string fileName in fileNames)
            {
                modules.AddRange(LoadModules(fileName));
            }

            foreach (IModule module in modules)
            {
                lock (_modules)
                {
                    _modules.Add(module);
                }
            }
        }

        private static List<IModule> LoadModules(string fileName)
        {
            List<IModule> modules = new List<IModule>();

            Assembly assembly = Assembly.LoadFrom(fileName);

            Type[] exportedTypes = assembly.GetExportedTypes();
            
            foreach(Type exportedType in exportedTypes)
            {
                object instance = Activator.CreateInstance(exportedType);

                if (instance is IModule)
                {
                    modules.Add((IModule)instance);
                }
            }

            return modules;
        }

        private static string[] GetModuleFileNames(Uri location)
        {
            return Directory.GetFiles(location.LocalPath, MODULE_FILENAME_PATTERN);
        }
    }
}
