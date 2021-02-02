using GODrive.Api;
using GODrive.Utils;
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
    public partial class FileDialog : MetroFramework.Forms.MetroForm
    {
        List<string> listFiles = new List<string>();
        List<string> fileName = new List<string>();
        FileApi fileApi;
        string pathUpload = "";
        string folderSelected = "";


        public FileDialog()
        {
            InitializeComponent();
            this.btnUpload.Enabled = false;
            this.listView.MultiSelect = true;

        }

        public FileDialog(string pathUpload)
        {
            InitializeComponent();
            this.pathUpload = pathUpload;
            this.btnOpen.Enabled = false;
            this.listView.MultiSelect = false;
            SettingFolderDialog();
        }

        private void SettingFolderDialog()
        {
            string pathIcon = Directory.GetCurrentDirectory().ToString().Split(new[] { "godrive-desktop" }, StringSplitOptions.None)[0] + @"godrive-desktop\folder_contacts_15440.ico";
            string path = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\GODrive\");
            listFiles.Clear();
            listView.Items.Clear();
            txtPath.Text = path;
            foreach (string item in Directory.GetDirectories(path))
            {
                imageList.Images.Add(System.Drawing.Icon.ExtractAssociatedIcon(pathIcon.ToString()));
                DirectoryInfo fi = new DirectoryInfo(item);
                listFiles.Add(fi.FullName);
                listView.Items.Add(fi.Name, imageList.Images.Count - 1);
            }
        }



        private void FileDialog_Load(object sender, EventArgs e)
        {
            fileApi = new FileApi();
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fileName.Count == 0 && this.pathUpload == "")
            {
                this.btnUpload.Enabled = false;
            }
            else
            {
                this.btnUpload.Enabled = true;
            }

            if (listView.FocusedItem != null)
            {
                for (int i = 0; i < listView.SelectedItems.Count; i++)
                {
                    fileName.Add(listFiles[listView.SelectedItems[i].Index]);
                }
            }

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            listFiles.Clear();
            listView.Items.Clear();
            using (FolderBrowserDialog fbd = new FolderBrowserDialog() { Description = "Select your path" })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = fbd.SelectedPath;
                    foreach (string item in Directory.GetFiles(fbd.SelectedPath))
                    {
                        imageList.Images.Add(System.Drawing.Icon.ExtractAssociatedIcon(item));
                        FileInfo fi = new FileInfo(item);
                        listFiles.Add(fi.FullName);
                        listView.Items.Add(fi.Name, imageList.Images.Count - 1);
                    }

                }
            }

        }

        private void txtPath_TextChanged(object sender, EventArgs e)
        {
            if (fileName.Count == 0 && this.pathUpload == "")
            {
                this.btnUpload.Enabled = false;
            }
            else
            {
                this.btnUpload.Enabled = true;
            }

        }

        private async void btnUpload_Click(object sender, EventArgs e)
        {
            btnOpen.Enabled = false;
            btnUpload.Enabled = false;
            if (this.pathUpload == "")
            {
                List<string> files = new List<string>();
                for (int i = 0; i < listView.SelectedItems.Count; i++)
                {
                    files.Add(listFiles[listView.SelectedItems[i].Index]);
                }

                var unique_items = new HashSet<string>(files);
                foreach (string file in unique_items)
                {
                    FileInfo fi = new FileInfo(file);
                    await fileApi.UploadFileRightClick(Path.GetFullPath(fi.FullName), "shared all");
                }

                listView.SelectedItems.Clear();
            }
            else
            {
                string folder = listFiles[listView.FocusedItem.Index].Split('\\')[listFiles[listView.FocusedItem.Index].Split('\\').Length - 1].ToLower();
                await fileApi.UploadFileRightClick(Path.GetFullPath(this.pathUpload), folder);
            }
            btnOpen.Enabled = true;
            btnUpload.Enabled = true;
        }
    }
}
