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
using Game.Language;
using SqlDataProvider.BaseClass;
using Game.Logic;
using Bussiness.Managers;

namespace Game.Launcher
{
    public partial class Loading : Form
    {

        internal static System.Windows.Forms.RichTextBox LogView;
        internal static bool flag;


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
            var log = LogProvider.Default;
            this.runmgr = new RunMgr();
            Web.Server.WebServer.Runmgr = this.runmgr;

            if (!Sql_DbObject.TryConnection())
            {
                flag = false;
                log.Error("Fail to connect to SQL!");
            }
            log.Info("Succeed to to connect to SQL!");

            if (!LanguageMgr.Load())
            {
                log.Error("Fail to load language");
                flag = false;
            }
            log.Info("Succeed to load language");

            if (!MapMgr.Init())
            {
                flag = false;
                log.Error("Fail to load map");
            }
            log.Info("Succeed to load map");

            if (!ItemMgr.Init())
            {
                flag = false;
                log.Error("Fail to load item");
            }
            log.Info("Succeed to load item");

            if (!PropItemMgr.Init())
            {
                flag = false;
                log.Error("Fail to load propitem");
            }
            log.Info("Succeed to load propitem");

            if (!BallMgr.Init())
            {
                flag= false;
                log.Error("Fail to load ball");
            }
            log.Info("Succeed to load ball");

            if (!DropMgr.Init())
            {
                flag = false;
                log.Error("Fail to load drop");
            }
            log.Info("Succeed to load drop");

            if (!NPCInfoMgr.Init())
            {
                flag = false;
                log.Error("Fail to load npc");
            }
            log.Info("Succeed to load npc");


            if (!ItemBoxMgr.Init())
            {
                flag = false;
                log.Error("Fail to load itembox");
            }
            log.Info("Succeed to load itembox");


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
