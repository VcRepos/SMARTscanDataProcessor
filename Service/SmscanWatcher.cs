using System;
using System.IO;

namespace SMARTscan_DataProcessor
{
    public class SmscanWatcher
    {
        #region Public Fields

        private readonly FileSystemWatcher _smartScanWatcher;
        public string WatchingDirectory { get; set; }

        #endregion

        #region Constructor

        public SmscanWatcher(string watchingPath)
        {
            WatchingDirectory = watchingPath;

            _smartScanWatcher = new FileSystemWatcher()
            {
                IncludeSubdirectories = true,
                Path = watchingPath,
            };

            _smartScanWatcher.Filter = "*zip";
            _smartScanWatcher.EnableRaisingEvents = true;
        }

        #endregion

        #region SMARTscan Watcher Events

        public event FileSystemEventHandler SourceFolderCreated
        {
            add { _smartScanWatcher.Created += value; }
            remove { _smartScanWatcher.Created -= value; }
        }

        public event FileSystemEventHandler SourceFolderDeleted
        {
            add { _smartScanWatcher.Deleted += value; }
            remove { _smartScanWatcher.Deleted -= value; }
        }

        public event FileSystemEventHandler SourceFolderChanged
        {
            add { _smartScanWatcher.Changed += value; }
            remove { _smartScanWatcher.Changed -= value; }
        }

        #endregion

    }
}
