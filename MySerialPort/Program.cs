using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MySerialPort
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form = new Form1();
            form.FormClosed += delegate (Object sender, FormClosedEventArgs e)
            {
                Application.Exit();
            };
            Application.Run(form);
        }
    }
}
