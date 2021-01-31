using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GODrive.Utils
{
    /// <summary>
    /// Maps a user file system path to the remote storage path and back. 
    /// </summary>
    /// <remarks>You will change methods of this class to map the user file system path to your remote storage path.</remarks>
    internal static class Mapping
    {
        /// <summary>
        /// Returns a remote storage URI that corresponds to the user file system path.
        /// </summary>
        /// <param name="userFileSystemPath">Full path in the user file system.</param>
        /// <returns>Remote storage URI that corresponds to the <paramref name="userFileSystemPath"/>.</returns>
        public static string MapPath(string userFileSystemPath)
        {
            // Get path relative to the virtual root.
            string relativePath = Helper.TrimEndingDirectorySeparator(userFileSystemPath).Substring(
                Helper.TrimEndingDirectorySeparator(GODrive.Provider.PersonalProvider.GetRootPath()).Length);

            string path = $"{Helper.TrimEndingDirectorySeparator("")}{relativePath}";
            return path;
        }

        /// <summary>
        /// Returns a user file system path that corresponds to the remote storage URI.
        /// </summary>
        /// <param name="remoteStorageUri">Remote storage URI.</param>
        /// <returns>Path in the user file system that corresponds to the <paramref name="remoteStorageUri"/>.</returns>
        public static string ReverseMapPath(string remoteStorageUri)
        {
            // Get path relative to the virtual root.
            string relativePath = Helper.TrimEndingDirectorySeparator(remoteStorageUri).Substring(
                Helper.TrimEndingDirectorySeparator("").Length);

            string path = $"{Helper.TrimEndingDirectorySeparator(GODrive.Provider.PersonalProvider.GetRootPath())}{relativePath}";
            return path;
        }
    }
}
