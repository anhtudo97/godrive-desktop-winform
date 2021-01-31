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
using Windows.Storage;

namespace GODrive
{
    public partial class UserProfile : MetroFramework.Forms.MetroForm
    {
        RestClient client = new RestClient("https://api.au.godmerch.com");
        private string infoPath = Helper.AppDataFilePath();
        Provider.PersonalProvider personalProvider;

        public UserProfile()
        {
            InitializeComponent();
        }

        public UserProfile(string token)
        {
            InitializeComponent();
            getUserProfile(token);
            loadFile();
        }

        protected virtual void OnInitialized(EventArgs e)
        {

            personalProvider = new Provider.PersonalProvider();
            //Provider.PersonalProvider.Unregister();
            personalProvider.Setup();
        }

        private async void getUserProfile(string token)
        {
            RestRequest request = new RestRequest("profile", Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + token);

            /* Response */
            IRestResponse response = await client.ExecuteAsync(request);
            JObject responseJson = JObject.Parse(response.Content);
            JObject dataUser = (JObject)responseJson["data"];
        }

        private void loadFile()
        {
            var jsonPath = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), infoPath);

            /* Open and write token to file*/
            FileStream fs = File.Open(jsonPath, FileMode.Open);
            StreamReader str = new StreamReader(fs);
            string json = str.ReadToEnd();
            UserModel user = JsonConvert.DeserializeObject<UserModel>(json);
            str.Close();
            fs.Close();

            bindingData(user);
        }

        private void bindingData(UserModel currentUser)
        {
            this.txtEmail.Text = currentUser.email;
            this.txtName.Text = currentUser.fullName;
            this.txtRole.Text = currentUser.role;
        }

        private void Role_Click(object sender, EventArgs e)
        {

        }

        private void UserProfile_Load(object sender, EventArgs e)
        {
            WinAPI.AnimateWindow(this.Handle, 1000, WinAPI.BLEND);
        }
    }
}
