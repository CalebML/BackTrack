using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackTrack
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

            //get admin privalages
            System.Security.Principal.WindowsPrincipal pricipal =
                     new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = pricipal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);

            if (!hasAdministrativeRight)
            {
                RunElevated(Application.ExecutablePath);
                //this.Close();
                Application.Exit();
            }
            else
            {
                //we have admin privalages
                Application.Run(new BackTrack());
            }
        }

        private static bool RunElevated(string fileName)
        {
            //MessageBox.Show("Run: " + fileName);
            System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo();
            processInfo.Verb = "runas";
            processInfo.FileName = fileName;
            try
            {
                System.Diagnostics.Process.Start(processInfo);
                return true;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                //Do nothing. Probably the user canceled the UAC window
            }
            return false;
        }
    }
}
