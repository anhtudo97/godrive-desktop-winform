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
    public partial class Login : MaterialSkin.Controls.MaterialForm
    {
        RestClient client = new RestClient("https://api.au.godmerch.com");

        private string infoPath = Helper.AppDataFilePath();

        string path1 = @"\godrive_app";

        string fullPath;

        bool isExpried { get; set; }

        public Login()
        {
            Helper.InstallMeOnStartUp();
            InitializeComponent();

            MaterialSkin.MaterialSkinManager skinManager = MaterialSkin.MaterialSkinManager.Instance;
            skinManager.AddFormToManage(this);
            skinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.LIGHT;
            skinManager.ColorScheme = new MaterialSkin.ColorScheme(MaterialSkin.Primary.Green900, MaterialSkin.Primary.BlueGrey900, MaterialSkin.Primary.Blue500, MaterialSkin.Accent.Orange700, MaterialSkin.TextShade.WHITE);
        }

        private void Login_Load(object sender, EventArgs e)
        {
            onMounted();
            WinAPI.AnimateWindow(this.Handle, 1000, WinAPI.BLEND);

            this.notifyIcon.Visible = true;
            this.notifyIcon.BalloonTipTitle = "God Drive";
            this.notifyIcon.Text = "God Drive";

            ContextMenu notificationContextMenu = new ContextMenu();
            notificationContextMenu.MenuItems.Add("Open", new EventHandler(Window_Activated));
            notificationContextMenu.MenuItems.Add("Close", new EventHandler(Window_Deactivated));

            this.notifyIcon.ContextMenu = notificationContextMenu;
        }

        private async void onMounted()
        {
            string token = await checkTokenToLogin();
            if (token.Length > 0) redirectPage(token);
        }

        private async void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string token = await checkTokenToLogin();
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
            string token = await checkTokenToLogin();
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

        private async Task<string> checkTokenToLogin()
        {
            var jsonPath = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), infoPath);
            if (!File.Exists(jsonPath))
            {
                return "";
            }
            /* Open and write token to file*/
            FileStream fs = File.Open(jsonPath, FileMode.Open);
            StreamReader str = new StreamReader(fs);
            string json = str.ReadToEnd();
            if (json != "")
            {
                UserModel user = JsonConvert.DeserializeObject<UserModel>(json);
                str.Close();
                fs.Close();
                await trackingTokenExpired(user.token);
                return !isExpried ? user.token : "";
            }
            else
            {
                return "";
            }

        }

        private async Task<bool> trackingTokenExpired(string token)
        {
            RestRequest request = new RestRequest("profile", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + token);

            /* Response */
            IRestResponse response = await client.ExecuteAsync(request);
            JObject responseJson = JObject.Parse(response.Content);
            string message = responseJson["message"].ToString();

            if (!response.IsSuccessful)
            {
                isExpried = true;
                showNotification();
                return true;
            }
            return false;

        }

        private void showNotification()
        {
            const string message = "Token is invalid or expried need to relogin";
            const string caption = "Notification";
            var result = MessageBox.Show(message, caption,
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);

        }


    }
}
