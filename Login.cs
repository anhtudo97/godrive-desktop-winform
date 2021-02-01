using GODrive.DTO;
using GODrive.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GODrive
{
    public partial class Login : MetroFramework.Forms.MetroForm
    {
        RestClient client = new RestClient("https://api.au.godmerch.com");

        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            WinAPI.AnimateWindow(this.Handle, 1000, WinAPI.BLEND);

            this.notifyIcon.Visible = true;
            this.notifyIcon.BalloonTipTitle = "God Drive";
            this.notifyIcon.Text = "God Drive";

            ContextMenu notificationContextMenu = new ContextMenu();
            notificationContextMenu.MenuItems.Add("Open", new EventHandler(Window_Activated));
            notificationContextMenu.MenuItems.Add("Close", new EventHandler(Window_Deactivated));

            this.notifyIcon.ContextMenu = notificationContextMenu;
        }

        private async void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string token = await Helper.CheckTokenToLogin();
            if (token.Length > 0)
            {
                UserProfile userProfilePage = new UserProfile();
                userProfilePage.Show();
            }
            else
            {
                Login mainWindow = new Login();
                mainWindow.Show();
            }
        }

        private async void Window_Activated(object sender, EventArgs e)
        {
            string token = await Helper.CheckTokenToLogin();
            if (token.Length > 0)
            {
                UserProfile userProfilePage = new UserProfile();
                userProfilePage.Show();
            }
            else
            {
                Login mainWindow = new Login();
                mainWindow.Show();
            }
        }


        private void Window_Deactivated(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Text;

            RestRequest request = new RestRequest("auth/sign-in", Method.POST);
            request.RequestFormat = RestSharp.DataFormat.Json;
            request.AddJsonBody(new { email = email, password = password });

            /* Response */
            IRestResponse response = await client.ExecuteAsync(request);
            JObject responseJson = JObject.Parse(response.Content);
            JObject dataUser = (JObject)responseJson["data"];
            string token = dataUser["token"].ToString();
            saveUser(dataUser);

            redirectPage(token);
        }

        private void saveUser(JObject user)
        {
            UserModel userModel = new UserModel();
            userModel.fullName = user["fullName"].ToString();
            userModel.email = user["email"].ToString();
            userModel.role = user["role"].ToString();
            userModel.token = user["token"].ToString();
            saveTokenJson(userModel);
        }

        private void redirectPage(string token)
        {
            this.Hide();
            UserProfile userProfile = new UserProfile(token);
            userProfile.Show();
        }

        private void saveTokenJson(UserModel user)
        {
            var dataFolder = Helper.AppDataDirectoryPath();
            var dataFile = Helper.AppDataFilePath();
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
            MessageBox.Show(dataFile);
            string json = JsonConvert.SerializeObject(user);
            FileStream fs = File.Create(dataFile);
            StreamWriter str = new StreamWriter(fs);

            str.Write(json);
            str.Flush();
            str.Close();
            fs.Close();
        }
    }
}
