using GODrive.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GODrive
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Helper.InstallMeOnStartUp();
            OnMounted();
        }

        static void OnMounted()
        {

            string token = Helper.CheckTokenToLogin().Result;

            if(token != "")
            {
                Application.Run(new UserProfile());
            }
            else
            {
                Application.Run(new Login());
            }
        }
    }
}
