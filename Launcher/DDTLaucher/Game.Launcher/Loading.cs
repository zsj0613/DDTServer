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
using Lsj.Util.Logs.WinForms;
using Lsj.Util;

namespace Game.Launcher
{
    public partial class Loading : Form
    {

        internal static LogView LogView;
        internal static bool flag = false;


        private delegate void ShowMain(RunMgr runmgr);
        public Loading()
        {
            InitializeComponent();
            LogView = new LogView();
            this.TopMost = true;
        }
        RunMgr runmgr;
        private void Init(Object sender, EventArgs e)
        {

            LogProvider.Default = new LogProvider(new LogConfig
            {
                UseMessageBox = true,
                LogView = LogView,
                UseLogView = true,
                FilePath = @".\log\",
                UseFile = true
            });
            var log = LogProvider.Default;
            this.runmgr = new RunMgr();
            Web.Server.WebServer.Runmgr = this.runmgr;

            new Thread(() =>
            {
                if (!Sql_DbObject.TryConnection())
                {
                    log.Error("Fail to connect to SQL!");
                    WinForm.Notice("初始化失败，请查看日志。");
                    return;
                }
                else
                {
                    log.Info("Succeed to to connect to SQL!");
                }
                if (!LanguageMgr.Load())
                {
                    log.Error("Fail to load language");
                    WinForm.Notice("初始化失败，请查看日志。");
                    return;
                }
                else
                {
                    log.Info("Succeed to load language");
                }
                if (!MapMgr.Init())
                {
                    log.Error("Fail to load map");
                    WinForm.Notice("初始化失败，请查看日志。");
                    return;
                }
                else
                {
                    log.Info("Succeed to load map");
                }
                if (!ItemMgr.Init())
                {
                    log.Error("Fail to load item");
                    WinForm.Notice("初始化失败，请查看日志。");
                    return;
                }
                else
                {
                    log.Info("Succeed to load item");
                }
                if (!PropItemMgr.Init())
                {
                    log.Error("Fail to load propitem");
                    WinForm.Notice("初始化失败，请查看日志。");
                    return;
                }
                else
                {
                    log.Info("Succeed to load propitem");
                }
                if (!BallMgr.Init())
                {
                    log.Error("Fail to load ball");
                    WinForm.Notice("初始化失败，请查看日志。");
                    return;
                }
                else
                {
                    log.Info("Succeed to load ball");
                }
                if (!DropMgr.Init())
                {
                    log.Error("Fail to load drop");
                    WinForm.Notice("初始化失败，请查看日志。");
                    return;
                }
                else
                {
                    log.Info("Succeed to load drop");
                }
                if (!NPCInfoMgr.Init())
                {
                    log.Error("Fail to load npc");
                    WinForm.Notice("初始化失败，请查看日志。");
                    return;
                }
                else
                {
                    log.Info("Succeed to load npc");
                }

                if (!ItemBoxMgr.Init())
                {
                    log.Error("Fail to load itembox");
                    WinForm.Notice("初始化失败，请查看日志。");
                    return;
                }
                else
                {
                    log.Info("Succeed to load itembox");
                }
                flag = true;
            }).Start();

            Thread thread = new Thread(run);
            thread.Start();

        }
        private void run()
        {

            this.Invoke(new ShowMain(Installed), runmgr);
        }
        private void Installed(RunMgr runmgr)
        {
            MainForm b = new MainForm(runmgr);
            b.Show();
            this.Hide();
        }




    }
}
