using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
namespace Game.Launcher
{
    public partial class Loading : Form
    {
        private delegate void ShowMain(RunMgr runmgr);
        public Loading()
        {
            InitializeComponent();
            this.TopMost = true;
        }
        RunMgr runmgr;
        private void Init(Object sender, EventArgs e)
        {
            Thread thread = new Thread(run);
            thread.Start();



        }
        private void run()
        {
            this.runmgr = new RunMgr();
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
