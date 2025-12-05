using Sportstore;
using System;
using System.Windows.Forms;

namespace SportAuthSystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm()); // Запускаем с формы входа
        }
    }
}
