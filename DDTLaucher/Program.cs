using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lsj.Util.Log;
using Lsj.Util;

namespace Game.Launcher
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            WinForm.CatchAll();
            Log.Default = new Log(new LogConfig { FilePath = "log/Laucher", UseConsole = false });
            Log.Default.Debug("Start Laucher!");
            


            #region 应用程序的主入口点
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Loading());

            #endregion

        }


    }
}
