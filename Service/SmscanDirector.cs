using System;
using System.IO;
using Topshelf;
using SMARTscan_DataProcessor.Data;

namespace SMARTscan_DataProcessor.Service
{
    public class SmscanDirector : ServiceControl
    {
        #region Private Fields

        private SmscanWatcher watcher;
        private SmscanBuilder builder;

        #endregion

        #region Constructor

        public SmscanDirector()
        {
            AppLogger.ConfigApp();
            watcher = new SmscanWatcher(Global.SMsourceFolder);
        }

        #endregion

        #region Public Methods

        #endregion

        #region Public Method

        public void Start()
        {
            watcher.SourceFolderCreated += new FileSystemEventHandler(SmscanCreated);
            Console.WriteLine("The program start watching!");
        }

        public void Stop()
        {
            watcher.SourceFolderCreated -= new FileSystemEventHandler(SmscanCreated);
            Console.WriteLine("The program stop watching!");
        }

        #endregion

        #region Events

        void SmscanCreated(object sender, FileSystemEventArgs e)
        {
            AppLogger.LogInformation($"Smartscan project:{e.Name} is detected");

            if (e == null)
            {
                AppLogger.LogInformation($"Smartscan project is a null project cannot be processed");
            }
            else
            {
                builder = new SmscanBuilder(e.FullPath);
                builder.BuildService();
            }
        }

        public bool Start(HostControl hostControl)
        {
            throw new NotImplementedException();
        }

        public bool Stop(HostControl hostControl)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
