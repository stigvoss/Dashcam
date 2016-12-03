using Library.Dashcam.Extensibility.Interfaces;
using Newtonsoft.Json;
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
                FileInfo fileInfo = new FileInfo(fileName);

                IEnumerable<IModule> loaded = LoadModules(fileName);

                foreach (IModule module in loaded)
                {
                    if (module.Configuration.Enable)
                    {
                        LoadConfigurations(module, fileInfo);
                        modules.Add(module);
                    }
                }
            }

            foreach (IModule module in modules)
            {
                lock (_modules)
                {
                    _modules.Add(module);
                }
            }
        }

        private static void LoadConfigurations(IModule module, FileInfo fileInfo)
        {
            const string CONFIGURATION_EXTENSION = ".json";

            DirectoryInfo directory = fileInfo.Directory;

            string fileName = fileInfo.Name;

            string baseFileName = Path.GetFileNameWithoutExtension(fileName);

            string configurationFile = Path.Combine(directory.FullName, baseFileName + CONFIGURATION_EXTENSION);

            Type configurationType = module.ConfigurationType;

            IConfiguration configuration = new DefaultConfiguration();

            if (configurationType != null)
            {
                configuration = (IConfiguration)Activator.CreateInstance(configurationType);

                if (File.Exists(configurationFile))
                {
                    string content = File.ReadAllText(configurationFile);

                    JsonConvert.PopulateObject(content, configuration);
                }
            }

            module.Configuration = configuration;
        }

        private static List<IModule> LoadModules(string fileName)
        {
            List<IModule> modules = new List<IModule>();

            Assembly assembly = Assembly.LoadFrom(fileName);

            Type[] exportedTypes = assembly.GetExportedTypes();

            foreach (Type exportedType in exportedTypes)
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
