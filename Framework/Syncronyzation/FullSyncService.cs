using ITHit.FileSystem;
using ITHit.FileSystem.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Provider;

namespace VirtualFileSystem.Syncronyzation
{
    /// <summary>
    /// Doing full synchronization between the user file system and the remote storage, recursively going through all folders.
    /// </summary>
    /// <remarks>
    /// This is a simple full synchronyzation example. 
    /// 
    /// You can use this class in your project out of the box or replace with a more advanced algorithm.
    /// </remarks>
    internal class FullSyncService : IDisposable
    {
        System.Timers.Timer timer = null;

        /// <summary>
        /// User file system path.
        /// </summary>
        private string userFileSystemRootPath;

        /// <summary>
        /// Creates instance of this class.
        /// </summary>
        /// <param name="syncIntervalMs">Synchronization interval in milliseconds.</param>
        /// <param name="userFileSystemRootPath">User file system root path.</param>
        /// <param name="log">Logger.</param>
        internal FullSyncService(double syncIntervalMs, string userFileSystemRootPath) : base()
        {
            timer = new System.Timers.Timer(syncIntervalMs);
            this.userFileSystemRootPath = userFileSystemRootPath;
        }

        /// <summary>
        /// Starts synchronization.
        /// </summary>
        internal async Task StartAsync()
        {
            timer.Elapsed += Timer_ElapsedAsync;

            // Do not start next synchronyzation automatically, wait until previous synchronyzation completed.
            timer.AutoReset = false; 
            timer.Start();

            Debug.WriteLine($"Started");
        }

        private async void Timer_ElapsedAsync(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                // UFS -> RS. Recursivery synchronize all moved files and folders present in the user file system. Restore OriginalPath.
                await new ClientToServerSync().SyncronizeMovedAsync(userFileSystemRootPath);

                // UFS -> RS. Recursivery synchronize all updated/created/deleted file and folders present in the user file system.
                await new ClientToServerSync().SyncronizeFolderAsync(userFileSystemRootPath);

                // UFS <- RS. Recursivery synchronize all updated/created/deleted file and folders present in the user file system.
                await new ServerToClientSync().SyncronizeFolderAsync(userFileSystemRootPath);

                // Wait and than start synchronyzation again.
                timer.Start();
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Timer failure:", null, null, ex);
                throw;
            }
        }

        private void Error(object sender, ErrorEventArgs e)
        {
            Debug.WriteLine(null, null, null, e.GetException());
        }

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    timer.Stop();
                    timer.Dispose();
                    Debug.WriteLine($"Disposed");
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SyncService()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

    }
}
