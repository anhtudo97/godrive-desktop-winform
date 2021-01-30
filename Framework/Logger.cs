using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace VirtualFileSystem
{
    /// <summary>
    /// Implements unified logging.
    /// </summary>
    internal class Logger
    {
        /// <summary>
        /// Name of the component that is writing to the log.
        /// </summary>
        private string componentName;

        /// <summary>
        /// Log4Net Logger.
        /// </summary>

        /// <summary>
        /// Creates instance of this class.
        /// </summary>
        /// <param name="componentName">Name of the component that is writing to the log.</param>
        /// <param name="logger">Log4Net Logger.</param>
        internal Logger(string compNamer)
        {
            this.componentName = compNamer;
        }

        /// <inheritdoc/>
        public void LogError(string message, string sourcePath = null, string targetPath = null, Exception ex = null)
        {
            string att = FsPath.Exists(sourcePath) ? FsPath.GetAttString(sourcePath) : null;
            Debug.WriteLine($"\n{DateTime.Now} [{Thread.CurrentThread.ManagedThreadId,2}] {componentName,-26}{message,-45} {sourcePath,-80} {att} ", ex);
        }

        /// <inheritdoc/>
        public void LogMessage(string message, string sourcePath = null, string targetPath = null)
        {
            string att = FsPath.Exists(sourcePath) ? FsPath.GetAttString(sourcePath) : null;
            string size = FsPath.Size(sourcePath);

            Debug.WriteLine($"\n{DateTime.Now} [{Thread.CurrentThread.ManagedThreadId,2}] {componentName,-26}{message,-45} {sourcePath,-80} {size,7} {att} {targetPath}");
        }
    }
}
