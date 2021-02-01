﻿using GODrive.DTO;
using IWshRuntimeLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GODrive.Utils
{
    class Helper
    {
        static RestClient client = new RestClient("https://api.au.godmerch.com");
        public static string AppDataDirectoryPath()
        {
            var infoPath = @"GODrive\";
            var jsonPath = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), infoPath);
            //if (!File.Exists(jsonPath)) jsonPath = Path.Combine(Environment.GetEnvironmentVariable("AppData"), infoPath);
            return jsonPath;
        }
        public static string AppDataFilePath()
        {
            var infoPath = @"GODrive\info.json";
            var jsonPath = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), infoPath);
            //if (!File.Exists(jsonPath)) jsonPath = Path.Combine(Environment.GetEnvironmentVariable("AppData"), infoPath);
            return jsonPath;
        }

        public static string GetUserToken()
        {
            var jsonPath = AppDataFilePath();
            if (!System.IO.File.Exists(jsonPath))
            {
                return "";
            }
            /* Open and write token to file*/
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

        public static async Task<string> CheckTokenToLogin()
        {
            string infoPath = Helper.AppDataFilePath();

            string jsonPath = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), infoPath);
            if (!System.IO.File.Exists(jsonPath))
            {
                return "";
            }
            /* Open and write token to file*/
            FileStream fs = System.IO.File.Open(jsonPath, FileMode.Open);
            StreamReader str = new StreamReader(fs);
            string json = str.ReadToEnd();
            if (json != "")
            {
                UserModel user = JsonConvert.DeserializeObject<UserModel>(json);
                str.Close();
                fs.Close();
                bool isExpried = await TrackingTokenExpired(user.token);
                return !isExpried ? user.token : "";
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
    }
}
