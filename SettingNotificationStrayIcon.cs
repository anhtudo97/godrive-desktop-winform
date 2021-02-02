using GODrive.Utils;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace GODrive
{
    class SettingNotificationStrayIcon
    {
        NotifyIcon m_notifyIcon;
        FileDialog fileDialog;
        public SettingNotificationStrayIcon()
        {
            this.fileDialog = new FileDialog();
            this.m_notifyIcon = new NotifyIcon();
            //this.fileDialog = fileDialog;
            //this.m_notifyIcon = m_notifyIcon;
        }
        public void settingNotification()
        {
            m_notifyIcon.Icon = new Icon(SystemIcons.Information, 40, 40);
            m_notifyIcon.Visible = true;
            m_notifyIcon.BalloonTipTitle = "God Drive";
            m_notifyIcon.Text = "God Drive";

            ContextMenu notificationContextMenu = new ContextMenu();
            notificationContextMenu.MenuItems.Add("Open", new EventHandler(Window_Activated));
            notificationContextMenu.MenuItems.Add("Open dialog", new EventHandler(openDialog));
            notificationContextMenu.MenuItems.Add("Close", new EventHandler(Window_Deactivated));

            m_notifyIcon.ContextMenu = notificationContextMenu;

            // Handle the DoubleClick event to activate the form.
            m_notifyIcon.DoubleClick += new System.EventHandler(notifyIcon1_DoubleClick);
        }

        private async void notifyIcon1_DoubleClick(object Sender, EventArgs e)
        {
            string token = await Helper.CheckTokenToLogin();

            if (token.Length > 0 )
            {
                UserProfile userProfilePage = new UserProfile();
                userProfilePage.Show();
            }
            else
            {
                Login login = new Login();
                login.ShowDialog();
            }
        }

        private async void Window_Activated(object sender, EventArgs e)
        {
            
            string token = await Helper.CheckTokenToLogin();
            //bool isExpire = Helper.TrackingTokenExpired(token).Result;
            Console.WriteLine("123" + token);

            if (token != "")
            {
                UserProfile userProfilePage = new UserProfile();
                userProfilePage.Show();
            }
            else
            {
                Login login = new Login();
                login.ShowDialog();
            }
        }

        private void openDialog(object sender, EventArgs e)
        {
            if (!fileDialog.Visible)
            {
                fileDialog.ShowDialog();
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}
