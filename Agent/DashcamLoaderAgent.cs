using Library.Dashcam.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Agent
{
    public partial class DashcamLoaderAgent : ServiceBase
    {
        private const string FILESYSTEM_FAT32 = "FAT32";

        private CancellationTokenSource _source;
        private CancellationToken _token;

        private TaskFactory _factory;
        private Task _task;

        public DashcamLoaderAgent()
        {
            InitializeComponent();

            _factory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);
        }

        protected override void OnStart(string[] args)
        {
            _task = _factory.StartNew(() => Loader(_token), _token);
        }

        protected override void OnStop()
        {
            _source.Cancel();
            _source.Dispose();
        }

        private void Loader(CancellationToken token)
        {
            bool hasInstallation = false;

            string app = null;

            while (!hasInstallation && !token.IsCancellationRequested)
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady && drive.DriveFormat.Equals(FILESYSTEM_FAT32))
                    {
                        string root = drive.RootDirectory.FullName;

                        string config = Path.Combine(root, Constants.FILE_NAME_CONFIGURATION);
                        string bin = Path.Combine(root, Constants.DIRECTORY_NAME_BINARIES);
                        app = Path.Combine(bin, "App.exe");
                        string data = Path.Combine(root, Constants.DIRECTORY_NAME_DATA);
                        string modules = Path.Combine(root, Constants.DIRECTORY_NAME_MODULES);

                        string[] directory = new string[]
                        {
                            bin,
                            data,
                            modules
                        };

                        string[] files = new string[]
                        {
                            config,
                            app
                        };

                        hasInstallation = directory.All(d => Directory.Exists(d)) && files.All(f => File.Exists(f));

                        break;
                    }
                }
            }


            if(!hasInstallation)
            {
                Assembly assembly = Assembly.LoadFrom(app);

                FileInfo file = new FileInfo(assembly.Location);

                Directory.SetCurrentDirectory(file.Directory.FullName);

                Type type = assembly.GetType("App.Program");

                object program = Activator.CreateInstance(type);

                MethodInfo start = type.GetMethod("Start");
                MethodInfo stop = type.GetMethod("Stop");

                start.Invoke(program, null);

                while (!token.IsCancellationRequested)
                {
                    Thread.Sleep(100);
                }

                stop.Invoke(program, null);
            }
        }
    }
}
