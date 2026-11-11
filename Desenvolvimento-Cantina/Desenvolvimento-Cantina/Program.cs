using Desenvolvimento_Cantina;
using System;
using System.Windows.Forms;

namespace Desenvolvimento_Cantina
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            Application.Run(new FrmProdutos());
        }
    }
}