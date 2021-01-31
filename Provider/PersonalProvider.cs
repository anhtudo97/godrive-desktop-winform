using GODrive.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.Provider;
using GODrive.DTO;
using GODrive.Utils;
using System.Windows.Forms;

namespace GODrive.Provider
{
    class PersonalProvider
    {
        static bool Watching;
        static string syncRootId = "GODrive!S-1-1234!Personal";
        static string ProviderName = "GODrive - Personal";
        static string RootWithEnv = @"%USERPROFILE%\GODrive\";
        static FileApi api = new FileApi();

        private FileSystemWatcher watcher { get; set; }
        public static string GetRootPath()
        {
            var rootFolderPath = Environment.ExpandEnvironmentVariables(RootWithEnv);
            return rootFolderPath;
        }

        public static string GetLocalPath(string path)
        {
            var rootFolderPath = Environment.ExpandEnvironmentVariables(RootWithEnv);
            return rootFolderPath + path.Replace("/", "\\");
        }

        public static string GetRemotePath(string path)
        {
            var rootFolderPath = Environment.ExpandEnvironmentVariables(RootWithEnv);
            string remotePath = path.Replace(rootFolderPath, "").Replace("\\", "/");
            return remotePath;
        }

        public async void Register()
        {
            string folderRoot = GetRootPath();
            if (!Directory.Exists(folderRoot))
            {
                Directory.CreateDirectory(folderRoot);
            }
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderRoot);
            StorageProviderSyncRootInfo info = new StorageProviderSyncRootInfo();

            info.Id = syncRootId;
            info.Path = folder;
            info.DisplayNameResource = ProviderName;
            info.AllowPinning = true;
            info.IconResource = "%SystemRoot%\\system32\\charmap.exe,0";
            info.HydrationPolicy = StorageProviderHydrationPolicy.Progressive;
            info.HydrationPolicyModifier = StorageProviderHydrationPolicyModifier.StreamingAllowed;
            info.PopulationPolicy = StorageProviderPopulationPolicy.AlwaysFull;
            info.InSyncPolicy = StorageProviderInSyncPolicy.FileCreationTime | StorageProviderInSyncPolicy.DirectoryCreationTime;
            info.Version = "1.0.0";
            info.ShowSiblingsAsGroup = false;
            info.HardlinkPolicy = StorageProviderHardlinkPolicy.None;
            info.Context = CryptographicBuffer.ConvertStringToBinary(folderRoot, BinaryStringEncoding.Utf8);

            var customStates = info.StorageProviderItemPropertyDefinitions;
            customStates.Add(new StorageProviderItemPropertyDefinition()
            {
                Id = 1,
                DisplayNameResource = "CustomStateName1"
            });
            customStates.Add(new StorageProviderItemPropertyDefinition()
            {
                Id = 2,
                DisplayNameResource = "CustomStateName2"
            });
            customStates.Add(new StorageProviderItemPropertyDefinition()
            {
                Id = 3,
                DisplayNameResource = "CustomStateName3"
            });

            StorageProviderSyncRootManager.Register(info);
            System.Threading.Thread.Sleep(1000);
        }

        public void Unregister()
        {
            StorageProviderSyncRootManager.Unregister(syncRootId);
        }

        public async void Setup()
        {
            if (!Watching)
            {
                await Task.Run(() => Register());
                if (!Directory.Exists(GetRootPath() + @"/Shared All"))
                {
                    Directory.CreateDirectory(GetRootPath() + @"/Shared All");
                }
                await Task.Run(() => SyncFolder("/shared all"));
            }
        }

        public async void SyncFiles()
        {
            string rootFolderPath = GetRootPath();
            List<Folder> folderResponse = await api.GetFolders("");
            for (int i = 0; i < folderResponse.Count; i++)
            {
                Folder folder = folderResponse[i];
                string folderPath = rootFolderPath + folder.Path.Replace("/", "\\");
                if (Helper.GetItemType(folder.Name) == "folder")
                {
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                }
                if (Helper.GetItemType(folder.Name) == "file")
                {
                    if (!File.Exists(folderPath))
                    {
                        api.DownloadFileEmpty(folder.Path, folderPath);
                    }
                }
            }
        }

        public async void SyncFolder(string path)
        {
            string rootFolderPath = GetRootPath();
            string fullPath = rootFolderPath + path.Replace("/", "\\");
            List<Folder> folderResponse = await api.GetFolders(path);

            // Create files/folders from remote server
            for (int i = 0; i < folderResponse.Count; i++)
            {
                Folder folder = folderResponse[i];
                string folderPath = rootFolderPath + folder.Path.Replace("/", "\\");
                if (Helper.GetItemType(folder.Name) == "folder")
                {
                    if (!Directory.Exists(folderPath))
                    {
                        Debug.WriteLine("Create directory: " + folderPath);
                        Directory.CreateDirectory(folderPath);
                    }

                }
                if (Helper.GetItemType(folder.Name) == "file")
                {
                    if (!File.Exists(folderPath))
                    {
                        api.DownloadFileEmpty(folder.Path, folderPath);
                    }
                }
            }

            // Delete folders that not exists on remote server
            string[] folders = Directory.GetDirectories(fullPath);
            for (int i = 0; i < folders.Length; i++)
            {
                string remotePath = GetRemotePath(folders[i]);
                MessageBox.Show(remotePath);
                Folder remoteItem = folderResponse.Find(f => f.Path.ToLower().Equals(remotePath.ToLower()));
                if (remoteItem == null)
                {
                    Directory.Delete(folders[i], true);
                }
            }

            // Delete file that not exist on remote server
            string[] files = Directory.GetFiles(fullPath);
            for (int i = 0; i < files.Length; i++)
            {
                string remotePath = GetRemotePath(files[i]);
                Folder remoteItem = folderResponse.Find(f => f.Path.ToLower().Equals(remotePath.ToLower()));
                if (remoteItem == null)
                {
                    File.Delete(files[i]);
                }
            }
        }
    }
}
