using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Game.Base;
using Lsj.Util.Logs;

namespace Game.Launcher
{
    public partial class Loading : Form
    {

        internal static System.Windows.Forms.RichTextBox LogView;


        private delegate void ShowMain(RunMgr runmgr);
        public Loading()
        {
            InitializeComponent();
            LogView = new System.Windows.Forms.RichTextBox();
            this.TopMost = true;
        }
        RunMgr runmgr;
        private void Init(Object sender, EventArgs e)
        {

            LogProvider.Default = new LogProvider(new LogConfig
            {
                UseMessageBox = true,
                RichTextBox = LogView,
                UseRichTextBox = true
            });
            this.runmgr = new RunMgr();
            Web.Server.WebServer.Runmgr = this.runmgr;




            Thread thread = new Thread(run);
            thread.Start();

        }
        private void run()
        {

            this.Invoke(new ShowMain(Installed),runmgr);
        }
        private void Installed(RunMgr runmgr)
        {
            MainForm b = new MainForm(runmgr);
            b.Show();
            this.Hide();
        }




    }
}
