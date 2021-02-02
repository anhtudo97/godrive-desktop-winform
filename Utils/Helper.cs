using GODrive.Api;
using GODrive.DTO;
using GODrive.Provider;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Storage;

namespace GODrive.Utils
{
    class Helper
    {
        static RestClient client = new RestClient("https://api.au.godmerch.com");
        static string RootWithEnv = @"%USERPROFILE%\GODrive\";
        public static string AppDataDirectoryPath()
        {
            var infoPath = @"GODrive\";
            var jsonPath = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), infoPath);
            return jsonPath;
        }
        public static string AppDataFilePath()
        {
            var infoPath = @"GODrive\info.json";
            var jsonPath = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), infoPath);
            return jsonPath;
        }

        public static string GetUserToken()
        {
            var jsonPath = AppDataFilePath();
            if (!System.IO.File.Exists(jsonPath))
            {
                return "";
            }
            /* Open and read file*/
            string json = System.IO.File.ReadAllText(jsonPath);
            if (json != "")
            {
                UserModel user = JsonConvert.DeserializeObject<UserModel>(json);
                return user.token;
            }
            else
            {
                return "";
            }
        }

        public static async Task<bool> VerifyToken(string token)
        {

            RestRequest request = new RestRequest("profile", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + token);

            /* Response */
            IRestResponse response = await client.ExecuteAsync(request);
            JObject responseJson = JObject.Parse(response.Content);

            string message = responseJson["message"].ToString();

            if (message.Equals("invalid or expired jwt"))
            {
                return false;
            }
            return true;
        }

        public static string GetItemType(string name)
        {
            string[] strArr = name.Split('.');
            if (strArr.Length > 1 && !name.Contains("@"))
            {
                return "file";
            }
            return "folder";
        }

        public static string TrimEndingDirectorySeparator(string path)
        {
            return path.TrimEnd(Path.DirectorySeparatorChar);
        }

        public static void InstallMeOnStartUp()
        {
            WshShell wshShell = new WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut;
            string startUpFolderPath =
              Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            // Create the shortcut
            shortcut =
              (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(
                startUpFolderPath + "\\" +
                Application.ProductName + ".lnk");

            shortcut.TargetPath = Application.ExecutablePath;
            shortcut.WorkingDirectory = Application.StartupPath;
            shortcut.Description = "Launch GOD drive";
            // shortcut.IconLocation = Application.StartupPath + @"\App.ico";
            shortcut.Save();
        }

        public static string CheckTokenToLogin1()
        {
            string jsonPath = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), AppDataFilePath());

            if (!System.IO.File.Exists(jsonPath))
            {
                return "";
            }
            /* Open and write token to file*/
            FileStream fs = new FileStream(jsonPath, FileMode.Open);
            StreamReader str = new StreamReader(fs);
            string json = str.ReadToEnd();
            if (json != "")
            {
                UserModel user = JsonConvert.DeserializeObject<UserModel>(json);
                str.Close();
                fs.Close();
                Console.WriteLine("tuanh");
                //bool isExpried = await TrackingTokenExpired(user.token);
                return user.token;
            }
            else
            {
                return "";
            }
        }

        public static async Task<string> CheckTokenToLogin()
        {
            string jsonPath = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), AppDataFilePath());

            if (!System.IO.File.Exists(jsonPath))
            {
                return "";
            }
            /* Open and write token to file*/
            FileStream fs = new FileStream(jsonPath, FileMode.Open);
            StreamReader str = new StreamReader(fs);
            string json = str.ReadToEnd();
            if (json != "")
            {
                UserModel user = JsonConvert.DeserializeObject<UserModel>(json);
                str.Close();
                fs.Close();
                bool isExpried = await TrackingTokenExpired(user.token);
                Console.WriteLine("tuanh");
                return !isExpried ? user.token : "";
                //return user.token;
            }
            else
            {
                return "";
            }
        }

        public static async Task<bool> TrackingTokenExpired(string token)
        {
            RestRequest request = new RestRequest("profile", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + token);

            /* Response */
            IRestResponse response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                ShowNotification();
                return true;
            }
            return false;

        }

        public static void ShowNotification()
        {
            const string message = "Token is invalid or expried need to relogin";
            const string caption = "Notification";
            var result = MessageBox.Show(message, caption,
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);

        }

        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void ReRun()
        {

            if (!IsAdministrator())
            {
                // Restart and run as admin
                var exeName = Process.GetCurrentProcess().MainModule.FileName;
                ProcessStartInfo startInfo = new ProcessStartInfo(exeName);
                startInfo.Verb = "runas";
                startInfo.Arguments = "restart";
                Process.Start(startInfo);
                System.Windows.Forms.Application.Exit();
            }
        }

        public static void RegistryRightClick()
        {
            string menuCommand = string.Format(
                    "\"{0}\" \"%L\"", System.Windows.Forms.Application.ExecutablePath);

            string menuCommandFolder = string.Format(
                   "\"{0}\" \"%V\"", System.Windows.Forms.Application.ExecutablePath);

            /* Registry right click on file */
            RegistryKey uploadFilekey = Registry.ClassesRoot.CreateSubKey(@"*\shell\Upload file");
            uploadFilekey.SetValue(null, "Upload file with GODrive");

            RegistryKey commandFilekey = Registry.ClassesRoot.CreateSubKey(@"*\shell\Upload file\command");
            commandFilekey.SetValue(null, menuCommand);

            /* Registry right click on folder */
            RegistryKey uploadFolderkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Classes\Directory\background\shell\Upload dialog");
            uploadFolderkey.SetValue(null, "Upload dialog with GODrive");
            uploadFolderkey.SetValue("Icon", System.Windows.Forms.Application.ExecutablePath);

            RegistryKey commandFolderkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Classes\Directory\background\shell\Upload dialog\command");
            commandFolderkey.SetValue(null, menuCommandFolder);
        }

        public static bool checkIsDir(string path)
        {
            System.IO.FileAttributes attr = System.IO.File.GetAttributes(path);
            if (attr.HasFlag(System.IO.FileAttributes.Directory))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void RegistryMultipleOptionUpload()
        {
            string uploadOtherCommand = string.Format(
                    "\"{0}\" \"%1 other-file\"", System.Windows.Forms.Application.ExecutablePath);

            string uploadRootCommand = string.Format(
                   "\"{0}\" \"%L\"", System.Windows.Forms.Application.ExecutablePath);


            /* Registry right click command upload */
            RegistryKey uploadOtherkey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\Upload.other");
            uploadOtherkey.SetValue("MUIVerb", "Upload other folder");
            RegistryKey uploadOtherCommandkey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\Upload.other\command");
            uploadOtherCommandkey.SetValue(null, uploadOtherCommand);

            RegistryKey uploadRootkey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\Upload.root");
            uploadRootkey.SetValue("MUIVerb", "Upload root folder");
            RegistryKey uploadRootCommandkey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell\Upload.root\command");
            uploadRootCommandkey.SetValue(null, uploadRootCommand);

            /* Registry right click options upload */
            RegistryKey uploadOptionkey = Registry.ClassesRoot.CreateSubKey(@"*\shell\Upload filw GD");
            uploadOptionkey.SetValue("icon", "imageres.dll,-5308");
            uploadOptionkey.SetValue("MUIVerb", "Upload file with GODrive");
            uploadOptionkey.SetValue("SubCommands", "Upload.root;Upload.other");
        }


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

        public static string GetUserFolderRoot()
        {
            var jsonPath = AppDataFilePath();
            if (System.IO.File.Exists(jsonPath))
            {
                /* Open and write token to file*/
                FileStream fs = System.IO.File.Open(jsonPath, FileMode.Open);
                StreamReader str = new StreamReader(fs);
                string json = str.ReadToEnd();
                UserModel user = JsonConvert.DeserializeObject<UserModel>(json);
                str.Close();
                fs.Close();

                return user.path;
            }

            return @"%USERPROFILE%\GODrive\";

        }

        public static string GetUserFolderUserName()
        {
            var jsonPath = AppDataFilePath();
            if (System.IO.File.Exists(jsonPath))
            {
                /* Open and write token to file*/
                FileStream fs = System.IO.File.Open(jsonPath, FileMode.Open);
                StreamReader str = new StreamReader(fs);
                string json = str.ReadToEnd();
                UserModel user = JsonConvert.DeserializeObject<UserModel>(json);
                str.Close();
                fs.Close();

                return user.email;
            }
            return "";
        }
    }
}
