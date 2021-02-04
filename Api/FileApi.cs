using GODrive.DTO;
using GODrive.Utils;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Storage;

namespace GODrive.Api
{
    class FileApi
    {
        const string BaseUrl = "https://api.au.godmerch.com";
        RestClient client;
        private string Token;

        public FileApi()
        {
            Token = Helper.GetUserToken();
            client = new RestClient(BaseUrl);
            client.Timeout = -1;
        }

        public async Task<List<Folder>> GetFolders(string path)
        {
            List<Folder> resFolders = new List<Folder>();
            client.Timeout = -1;
            var request = new RestRequest("/files/list_folder", Method.POST);
            request.AddHeader("Authorization", "Bearer " + Token);
            request.AddJsonBody(new
            {
                path = path,
                recursive = false,
                includeDeleted = false
            });
            IRestResponse response = await client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                FolderResponse res = JsonConvert.DeserializeObject<FolderResponse>(response.Content);
                resFolders.AddRange(res.Data.Entries);
                if (res.Data.HasMore)
                {
                    var cursor = res.Data.Cursor;
                    while (true)
                    {
                        var continueClient = new RestClient(BaseUrl + "/files/list_folder/continue");
                        var continueRequest = new RestRequest(Method.POST);
                        continueRequest.AddHeader("Authorization", "Bearer " + Token);
                        continueRequest.AddJsonBody(new
                        {
                            cursor = cursor
                        });
                        IRestResponse continueResponse = await continueClient.ExecuteAsync(continueRequest);
                        if (continueResponse.IsSuccessful)
                        {
                            FolderResponse continueRes = JsonConvert.DeserializeObject<FolderResponse>(continueResponse.Content);
                            resFolders.AddRange(continueRes.Data.Entries);
                            if (continueRes.Data.HasMore)
                            {
                                cursor = continueRes.Data.Cursor;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Get files to sync failed, err: " + continueResponse.Content);
                            break;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Get files to sync failed, err: " + response.Content);
            }
            return resFolders;
        }

        public void DownloadFile(string fileRemotePath, string fileLocalPath)
        {
            var request = new RestRequest("/files/download", Method.POST);
            request.AddHeader("Authorization", "Bearer " + Token);
            request.AddJsonBody(new
            {
                path = fileRemotePath
            });

            var writer = File.OpenWrite(fileLocalPath);

            request.ResponseWriter = responseStream =>
            {
                using (responseStream)
                {
                    responseStream.CopyTo(writer);
                }
            };

            var response = client.DownloadData(request);
        }

        public async void UpdateFileName(string currentPath, string newPath)
        {
            var request = new RestRequest("/files/move_v2", Method.POST);
            request.AddHeader("Authorization", "Bearer " + Token);
            request.AddJsonBody(new
            {
                from_path = currentPath,
                to_path = newPath,
                allow_shared_folder = false,
                autorename = false,
                allow_ownership_transfer = false
            });
            IRestResponse response = await client.ExecuteAsync(request);
            if (!response.IsSuccessful)
            {
                MessageBox.Show("Update filename failed, old file: " + currentPath + ", new path: " + newPath);
            }
        }

        public void UploadFile(string fileLocalPath)
        {
            var request = new RestRequest("/files/upload", Method.POST);
            request.AddHeader("Authorization", "Bearer " + Token);

            string remotePath = Helper.GetRemotePath(fileLocalPath).ToLower();
            if (!remotePath.StartsWith("/"))
            {
                remotePath = "/" + remotePath;
            }

            request.AddFile("file", fileLocalPath);
            request.AddParameter("path", remotePath);

            IRestResponse response = client.Execute(request);
            if (!response.IsSuccessful)
            {
                MessageBox.Show("Upload file failed, file: " + remotePath + "" + response.Content + "\nReason: " + response.ErrorMessage);
                Debug.WriteLine(JsonConvert.SerializeObject(response));
            }
        }

        public void UploadFileRightClick(string fileLocalPath, string folderUpload)
        {
            string[] filePaths = fileLocalPath.Split('\\');
            var request = new RestRequest("/files/upload", Method.POST);

            request.AddHeader("Authorization", "Bearer " + Token);

            request.AddFile("file", fileLocalPath);
            request.AddParameter("path", "/" + folderUpload.ToLower() + "/" + filePaths[filePaths.Length - 1].ToLower());

            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                MessageBox.Show("Upload file failed, file: " + "/shared all/" + filePaths[filePaths.Length - 1] + " " + response.Content + "\nReason: " + response.ErrorMessage);
                Debug.WriteLine(JsonConvert.SerializeObject(response));
            }
            else
            {
                MessageBox.Show("Upload file: " + "/shared all/" + filePaths[filePaths.Length - 1] );
            }

        }


        public async void DeleteFile(string filePath)
        {
            var request = new RestRequest("/files/delete_v2", Method.POST);
            request.AddHeader("Authorization", "Bearer " + Token);
            request.AddJsonBody(new
            {
                path = filePath
            });
            IRestResponse response = await client.ExecuteAsync(request);
            if (!response.IsSuccessful)
            {
                MessageBox.Show("Delete file failed, file: " + filePath);
            }
        }

        public async void DownloadFileEmpty(string fileRemotePath, string fileLocalPath)
        {
            void FileGrabber(StreamedFileDataRequest streamRequest)
            {
                try
                {
                    using (var writer = streamRequest.AsStreamForWrite())
                    {
                        var request = new RestRequest("/files/download", Method.POST);
                        request.AddHeader("Authorization", "Bearer " + Token);
                        request.AddJsonBody(new
                        {
                            path = fileRemotePath.ToLower()
                        });

                        request.ResponseWriter = responseStream =>
                        {
                            using (responseStream)
                            {
                                responseStream.CopyTo(writer);
                            }
                        };

                        client.DownloadData(request);
                    }
                    streamRequest.Dispose();
                }
                catch (Exception ex)
                {
                    streamRequest.FailAndClose(StreamedFileFailureMode.Incomplete);
                }
            }

            string fileName = Path.GetFileName(fileLocalPath);
            string folderPath = Path.GetDirectoryName(fileLocalPath);
            string fileExt = Path.GetExtension(fileLocalPath);
            Debug.WriteLine("Download remote file: " + fileRemotePath + ", " + fileLocalPath);
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            StorageFile file = await StorageFile.CreateStreamedFileAsync(fileName, FileGrabber, null);
            if (!File.Exists(fileLocalPath))
            {
                await file.CopyAsync(folder);
            }
        }
    }
}
