using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFramework.Factory
{
    /// <summary>
    /// 自动适配器自动更新
    /// </summary>
    internal class AdapterAutoUpdater
    {
        /// <summary>
        /// 
        /// </summary>
        private List<FileSystemWatcher> _watcher = new List<FileSystemWatcher>();
        private List<string> _allAdapterFiles = null;

        /// <summary>
        /// 
        /// </summary>
        public AdapterAutoUpdater()
        {
            _allAdapterFiles = DataAdapterConfigReader.GetAdapterFiles();

            List<string> targetDir = GetTargetDir(_allAdapterFiles);

            foreach (string dir in targetDir)
            {
                FileSystemWatcher watcher = new FileSystemWatcher(dir);
                watcher.EnableRaisingEvents = true;
                watcher.Changed += new FileSystemEventHandler(watcher_Changed);
            }
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                string file = e.FullPath;

                if (_allAdapterFiles.Contains(file))
                {
                    DataAdapterReflector.RefreshAssembly(file);
                }
            }
        }

        private List<string> GetTargetDir(List<string> allFiles)
        {
            List<string> lstDir = new List<string>();

            foreach (string fi in allFiles)
            {
                string dir = Path.GetDirectoryName(fi);

                if (!lstDir.Contains(dir))
                {
                    lstDir.Add(dir);
                }
            }

            return lstDir;
        }
    }
}
