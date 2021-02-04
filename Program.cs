using GODrive.Api;
using GODrive.Utils;
using Squirrel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GODrive
{
    static class Program
    {
        static NotifyIcon m_notifyIcon;
        static FileDialog fileDialog;
        static FileApi fileApi = new FileApi();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //UpdateApplication();

            OnMounted();
        }

        async static void UpdateApplication()
        {
            string[] pathProject = Environment.CurrentDirectory.ToString().Split(new string[] { "godrive-desktop" }, StringSplitOptions.None);
            using (var mgr = new UpdateManager(pathProject[0] + @"\godrive-desktop\"))
            {
                await mgr.UpdateApp();
            }
        }

        static void OnMounted()
        {
            string[] args = Environment.GetCommandLineArgs();

            string token = Helper.CheckTokenToLogin1();
            bool isExpire = Helper.TrackingTokenExpired(token).Result;
            SettingTrayIcon();

            if (args.Length > 1)
            {
                string[] filePath = args[1].Split(' ');

                if (!Helper.checkIsDir(filePath[0]))
                {
                    if (args[1].Contains("other-file"))
                    {
                       
                        Application.Run(new FileDialog(filePath[0]));
                    }
                    else
                    {
                        SaveFile((filePath[0]));
                        return;
                    }
                }
                else
                {
                    Application.Run(new FileDialog());
                }
            }
            else
            {
                if (token != "")
                {
                    Application.Run(new UserProfile());
                }
                else
                {
                    Application.Run(new Login());
                }
            }
        }

        static void SaveFile(string path)
        {
            fileApi.UploadFileRightClick(path, "shared all");
            Environment.Exit(-1);
        }

        static void SettingTrayIcon()
        {
            //fileDialog = new FileDialog();
            //m_notifyIcon = new NotifyIcon();

            SettingNotificationStrayIcon strayIcon = new SettingNotificationStrayIcon();
            strayIcon.settingNotification();
        }
    }
}
