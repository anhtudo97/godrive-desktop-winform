using GODrive.DTO;
using GODrive.Provider;
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

        public UserProfile()
        {
            InitializeComponent();
            loadFile();
        }


        private void loadFile()
        {
            var jsonPath = Helper.AppDataFilePath();

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

        private void UserProfile_Load(object sender, EventArgs e)
        {
            WinAPI.AnimateWindow(this.Handle, 1000, WinAPI.BLEND);

            //PersonalProvider.Setup();
        }

        private void UserProfile_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            this.Hide();
        }

        private void UserProfile_Resize(object sender, EventArgs e)
        {

        }
    }
}
