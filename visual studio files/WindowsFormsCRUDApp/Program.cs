using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManagerInventar
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //show login form first
            LoginForm loginForm = new LoginForm();
            DialogResult loginResult = loginForm.ShowDialog();

            //check if login was successful
            if (loginResult == DialogResult.OK && loginForm.LoginSuccessful)
            {
                //get user information from login form
                string username = loginForm.LoggedInUsername;
                string userRole = loginForm.UserRole;

                //close login form
                loginForm.Dispose();

                //create and show main form with user information
                MainForm mainForm = new MainForm(username, userRole);
                
                //run the main application loop
                Application.Run(mainForm);
            }
            else
            {
                //user cancelled login or login failed
                //application will exit
                if (loginForm != null)
                {
                    loginForm.Dispose();
                }
            }
        }
    }
}