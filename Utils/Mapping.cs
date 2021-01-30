﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualFileSystem;

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
                Helper.TrimEndingDirectorySeparator(GodDrive.Provider.PersonalProvider.GetRootPath()).Length);

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

            string path = $"{Helper.TrimEndingDirectorySeparator(GodDrive.Provider.PersonalProvider.GetRootPath())}{relativePath}";
            return path;
        }

        /// <summary>
        /// Gets a user file system item info from the remote storage data.
        /// </summary>
        /// <param name="remoteStorageItem">Remote storage item info.</param>
        /// <returns>User file system item info.</returns>
        public static FileSystemItemBasicInfo GetUserFileSysteItemBasicInfo(FileSystemInfo remoteStorageItem)
        {
            FileSystemItemBasicInfo userFileSystemItem;

            if (remoteStorageItem is FileInfo)
            {
                userFileSystemItem = new FileBasicInfo();
            }
            else
            {
                userFileSystemItem = new FolderBasicInfo();
            }

            userFileSystemItem.Name = remoteStorageItem.Name;
            userFileSystemItem.Attributes = remoteStorageItem.Attributes;
            userFileSystemItem.CreationTime = remoteStorageItem.CreationTime;
            userFileSystemItem.LastWriteTime = remoteStorageItem.LastWriteTime;
            userFileSystemItem.LastAccessTime = remoteStorageItem.LastAccessTime;
            userFileSystemItem.ChangeTime = remoteStorageItem.LastWriteTime;

            // You will send the ETag to 
            // the server inside If-Match header togeter with updated content from client.
            // This will make sure the changes on the server is not overwritten.
            //
            // In this sample, for the sake of simplicity, we use file last write time instead of ETag.
            userFileSystemItem.ETag = remoteStorageItem.LastWriteTime.ToBinary().ToString();

            // If the item is locked by another user, set the LockedByAnotherUser to true.
            // Here we just use the read-only attribute from remote storage item for demo purposes.
            userFileSystemItem.LockedByAnotherUser = (remoteStorageItem.Attributes & System.IO.FileAttributes.ReadOnly) != 0;

            // If the file is moved/renamed and the app is not running this will help us 
            // to sync the file/folder to remote storage after app starts.
            userFileSystemItem.CustomData = new CustomData
            {
                OriginalPath = Mapping.ReverseMapPath(remoteStorageItem.FullName)
            }.Serialize();

            if (remoteStorageItem is FileInfo)
            {
                ((FileBasicInfo)userFileSystemItem).Length = ((FileInfo)remoteStorageItem).Length;
            };

            return userFileSystemItem;
        }
    }
}
