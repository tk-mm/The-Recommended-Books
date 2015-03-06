using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebInfo
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DialogResult res;
            Form2 f2 = new Form2();
            res = f2.ShowDialog();
            f2.Dispose();

            if (res == DialogResult.OK)
            {
                Application.Run(new Form1());                
            }
        }
    }
}
