using BrandedClothingShop.Forms;
using System;
using System.Windows.Forms;

namespace BrandedClothingShop
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            try
            {
                Application.Run(new LoginForm());
            }
            finally
            {
                
                Application.Exit();
                Environment.Exit(0);
            }
        }
    }
}