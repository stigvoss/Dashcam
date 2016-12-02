using Accord.Video.DirectShow;
using Library.Dashcam.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Video;
using System.Collections.Concurrent;
using System.Threading;
using System.IO;
using Accord.Video.FFMPEG;
using System.Drawing;
using Toolbox.Extensions;
using Library.Dashcam.Extensibility;
using Library.Dashcam.Extensibility.Interfaces;

namespace Library.Dashcam.Core
{
    public class Core
    {
        private const int FRAME_BUFFER_SIZE = 120;

        private BlockingCollection<FrameInfo> _preProcess = new BlockingCollection<FrameInfo>(FRAME_BUFFER_SIZE);
        private BlockingCollection<FrameInfo> _frames = new BlockingCollection<FrameInfo>(FRAME_BUFFER_SIZE);

        private TaskFactory _factory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);

        private Task _processor;
        private Task _preprocessor;

        private readonly Configuration _configuration;
        private readonly Uri _data;
        private string _device;

        private VideoCaptureDevice _source;

        public delegate void CoreErrorEventHandler(object sender, EventArgs args);

        public event CoreErrorEventHandler OnError;

        public Configuration Configuration
        {
            get
            {
                return _configuration;
            }
        }

        public string Data
        {
            get
            {
                return _data.OriginalString;
            }
        }

        public string Device
        {
            get
            {
                return _device;
            }
        }

        public Core(Configuration configuration, string data)
        {
            _configuration = configuration;
            _data = new Uri(data);

            Initialize();
        }

        public IEnumerable<IModule> LoadModules(string modules)
        {
            List<IModule> loadedModules = new List<IModule>();

            ModuleLoader.Load(modules);

            loadedModules.AddRange(ModuleLoader.Modules);

            return loadedModules;
        }

        private void Initialize()
        {
            FilterInfo device = GetDevice(_configuration.Device);

            _device = device.Name;

            _source = new VideoCaptureDevice(device.MonikerString);

            _source.VideoResolution = GetDesiredCapability(_source, _configuration);
            _source.NewFrame += OnNewFrame;
            _source.VideoSourceError += OnVideoSourceError;
        }

        private VideoCapabilities GetDesiredCapability(VideoCaptureDevice device, Configuration configuration)
        {
            int width = configuration.Width;
            int height = configuration.Height;

            foreach (VideoCapabilities capability in device.VideoCapabilities)
            {
                Size frameSize = capability.FrameSize;

                if (frameSize.Width == width && frameSize.Height == height)
                {
                    return capability;
                }
            }

            throw new PlatformNotSupportedException("Resolution not supported by capture device.");
        }

        private void Process(BlockingCollection<FrameInfo> buffer)
        {
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

            VideoFileWriter writer = null;
            DateTime? begin = null;

            try
            {
                foreach (FrameInfo frameInfo in buffer.GetConsumingEnumerable())
                {
                    if (!begin.HasValue)
                    {
                        begin = frameInfo.Time;
                    }

                    if (writer == null)
                    {
                        long freeSpace = _data.GetDriveInfo().AvailableFreeSpace;
                        long maxRequiredSpace = (long)_configuration.BitRate * _configuration.ClipLength / 8;

                        if (maxRequiredSpace > freeSpace)
                        {
                            MakeFreeSpace(_data, maxRequiredSpace);
                        }

                        writer = new VideoFileWriter();
                        string fileName = $"{DateTime.Now.ToString(_configuration.FileNameFormat)}.{_configuration.FileFormat}";
                        writer.Open(Path.Combine(_data.LocalPath, fileName),
                        _source.VideoResolution.FrameSize.Width,
                        _source.VideoResolution.FrameSize.Height,
                        _source.VideoResolution.AverageFrameRate,
                        (VideoCodec)_configuration.Codec,
                        _configuration.BitRate,
                        AudioCodec.None, 0, 0, 0);
                    }
                    TimeSpan elapsed = frameInfo.Time - begin.Value;

                    writer.WriteVideoFrame(frameInfo.Frame, elapsed);

                    frameInfo.Dispose();

                    if (elapsed.TotalSeconds > _configuration.ClipLength)
                    {
                        begin = null;
                        writer.Flush();
                        writer.Dispose();
                        writer = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Flush();
                    writer.Dispose();
                }
            }
        }

        private void MakeFreeSpace(Uri uri, long targetFreeSpace)
        {
            DriveInfo drive = uri.GetDriveInfo();
            long availableSpace = drive.AvailableFreeSpace;

            if (!uri.IsFile && drive.AvailableFreeSpace <= targetFreeSpace)
            {
                DirectoryInfo directory = uri.GetDirectoryInfo();

                IEnumerable<FileInfo> files = directory.GetFiles();

                if (files.Count() > 0)
                {
                    files = files.OrderByDescending(file => file.LastWriteTimeUtc);

                    while (drive.AvailableFreeSpace < targetFreeSpace)
                    {
                        FileInfo file = files.FirstOrDefault();

                        if (file != null)
                        {
                            File.Delete(file.FullName);
                        }
                    }
                }
            }
        }

        private void OnVideoSourceError(object sender, VideoSourceErrorEventArgs eventArgs)
        {
            OnError?.Invoke(sender, eventArgs);
        }

        private void OnNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap;

            using (Bitmap frame = eventArgs.Frame)
            {
                bitmap = frame.DeepCopy();
            }

            if (!_preProcess.IsAddingCompleted)
            {
                try
                {
                    _preProcess.Add(new FrameInfo { Time = DateTime.Now, Frame = bitmap });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private FilterInfo GetDevice(int device)
        {
            FilterInfoCollection devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (device < 0 || devices.Count <= device)
                throw new ArgumentException("Invalid device");

            return devices[device];
        }

        public void Start()
        {
            _source.Start();
            _preprocessor = _factory.StartNew(() => PreProcess(_preProcess, _frames));
            _processor = _factory.StartNew(() => Process(_frames));
        }

        private void PreProcess(BlockingCollection<FrameInfo> preProcess, BlockingCollection<FrameInfo> process)
        {
            try
            {
                foreach (FrameInfo frameInfo in preProcess.GetConsumingEnumerable())
                {
                    foreach (IModule module in ModuleLoader.Modules)
                    {
                        if (module.Capabilities.Contains(ModuleCapabilities.MODIFY))
                        {
                            module.Execute(frameInfo);
                            process.Add(frameInfo);
                        }
                        
                        if (module.Capabilities.Contains(ModuleCapabilities.PROCESS))
                        {
                            module.Execute(new FrameInfo
                            {
                                Frame = frameInfo.Frame.DeepCopy(),
                                Time = frameInfo.Time
                            });
                            process.Add(frameInfo);
                        }

                        if (module.Capabilities.Contains(ModuleCapabilities.CONSUME))
                        {
                            module.Execute(frameInfo);
                        }
                    }
                }
            }
            finally
            {
                process.CompleteAdding();
            }
        }

        public void Stop()
        {
            _source.SignalToStop();
            _source.WaitForStop();

            _preProcess.CompleteAdding();
            _processor.Wait();
        }
    }
}
