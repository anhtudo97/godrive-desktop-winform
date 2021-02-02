using GODrive.Api;
using GODrive.Utils;
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
            OnMounted();
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
                        SaveFile(args[1]);
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
                if (token != "" && !isExpire)
                {
                    Application.Run(new UserProfile());
                }
                else
                {
                    Application.Run(new Login());
                }
            }
        }

        static async void SaveFile(string path)
        {
            await fileApi.UploadFileRightClick(path, "shared all");
            Application.Exit();
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
