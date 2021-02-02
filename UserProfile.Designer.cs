
namespace GODrive
{
    partial class UserProfile
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserProfile));
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.Role = new MetroFramework.Controls.MetroLabel();
            this.txtEmail = new MetroFramework.Controls.MetroLabel();
            this.txtName = new MetroFramework.Controls.MetroLabel();
            this.txtRole = new MetroFramework.Controls.MetroLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel1.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel1.Location = new System.Drawing.Point(21, 16);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(58, 25);
            this.metroLabel1.TabIndex = 1;
            this.metroLabel1.Text = "Email";
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel2.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.metroLabel2.Location = new System.Drawing.Point(21, 69);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(62, 25);
            this.metroLabel2.TabIndex = 3;
            this.metroLabel2.Text = "Name";
            // 
            // Role
            // 
            this.Role.AutoSize = true;
            this.Role.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.Role.FontWeight = MetroFramework.MetroLabelWeight.Bold;
            this.Role.Location = new System.Drawing.Point(21, 121);
            this.Role.Name = "Role";
            this.Role.Size = new System.Drawing.Size(50, 25);
            this.Role.TabIndex = 5;
            this.Role.Text = "Role";
            // 
            // txtEmail
            // 
            this.txtEmail.AutoSize = true;
            this.txtEmail.Location = new System.Drawing.Point(114, 19);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(41, 19);
            this.txtEmail.TabIndex = 6;
            this.txtEmail.Text = "Email";
            // 
            // txtName
            // 
            this.txtName.AutoSize = true;
            this.txtName.Location = new System.Drawing.Point(114, 72);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(41, 19);
            this.txtName.TabIndex = 7;
            this.txtName.Text = "Email";
            // 
            // txtRole
            // 
            this.txtRole.AutoSize = true;
            this.txtRole.Location = new System.Drawing.Point(114, 124);
            this.txtRole.Name = "txtRole";
            this.txtRole.Size = new System.Drawing.Size(41, 19);
            this.txtRole.TabIndex = 8;
            this.txtRole.Text = "Email";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(23, 83);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtEmail);
            this.panel1.Controls.Add(this.metroLabel1);
            this.panel1.Controls.Add(this.txtRole);
            this.panel1.Controls.Add(this.metroLabel2);
            this.panel1.Controls.Add(this.txtName);
            this.panel1.Controls.Add(this.Role);
            this.panel1.Location = new System.Drawing.Point(191, 63);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(272, 173);
            this.panel1.TabIndex = 10;
            // 
            // UserProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 299);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "UserProfile";
            this.Resizable = false;
            this.Text = "User Profile";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UserProfile_FormClosing);
            this.Load += new System.EventHandler(this.UserProfile_Load);
            this.Resize += new System.EventHandler(this.UserProfile_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroLabel Role;
        private MetroFramework.Controls.MetroLabel txtEmail;
        private MetroFramework.Controls.MetroLabel txtName;
        private MetroFramework.Controls.MetroLabel txtRole;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
    }
}