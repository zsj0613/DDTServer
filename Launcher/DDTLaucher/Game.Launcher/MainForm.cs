using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;
using Game.Base;
using Lsj.Util.Logs;
using Lsj.Util;

namespace Game.Launcher
{
    public partial class MainForm : Form
    {
        public MainForm(RunMgr runmgr)
        {
            InitializeComponent();
            this.runmgr = runmgr;

        }


        RunMgr runmgr;
        private bool IsLoaded = false;


        private void Form1_Load(object sender, EventArgs e)
        {
            IsLoaded = true;
            
            CheckStatus();
            NotifyIcon.ContextMenuStrip = NotifyMenuStrip;
            this.FixedSize();

                CenterButton.Enabled = false;
                FightButton.Enabled = false;
                GameButton.Enabled = false;
                WebButton.Enabled = false;
                LauncherButton.Enabled = false;


        }



        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            if (IsLoaded)
           this.CheckStatus();
        }

        private void CheckStatus()
        {
            if (Loading.flag == true)
            {

                CenterButton.Enabled = true;
                FightButton.Enabled = true;
                GameButton.Enabled = true;
                WebButton.Enabled = true;
                LauncherButton.Enabled = true;
                if (runmgr.UpdateStatus())
                {
                    if (runmgr.CenterStatus)
                    {
                        CenterButton.Text = "结束中心服务端";
                        CenterButton.ForeColor = Color.Red;
                    }
                    else
                    {
                        CenterButton.Text = "启动中心服务端";
                        CenterButton.ForeColor = Color.Green;
                    }



                    if (runmgr.FightStatus)
                    {
                        FightButton.Text = "结束战斗服务端";
                        FightButton.ForeColor = Color.Red;
                    }
                    else
                    {
                        FightButton.Text = "启动战斗服务端";
                        FightButton.ForeColor = Color.Green;
                    }



                    if (runmgr.GameStatus)
                    {
                        GameButton.Text = "结束游戏服务端";
                        GameButton.ForeColor = Color.Red;
                    }
                    else
                    {
                        GameButton.Text = "启动游戏服务端";
                        GameButton.ForeColor = Color.Green;
                    }
                    if (runmgr.WebStatus)
                    {
                        WebButton.Text = "结束WEB服务";
                        WebButton.ForeColor = Color.Red;
                    }
                    else
                    {
                        WebButton.Text = "启动WEB服务";
                        WebButton.ForeColor = Color.Green;
                    }


                    if (runmgr.IsAllRun)
                    {
                        LauncherButton.Text = "一键结束";
                        LauncherButton.ForeColor = Color.Red;
                    }
                    else
                    {
                        LauncherButton.Text = "一键启动";
                        LauncherButton.ForeColor = Color.Green;
                    }



                }
                else
                {
                    MessageBox.Show("获取状态错误！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            

        }








        private void CenterButton_Click(object sender, EventArgs e)
        {
            CheckStatus();
            if (runmgr.CenterStatus && MessageBox.Show("是否退出？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (!runmgr.StopCenter())
                {
                    MessageBox.Show("结束Center出错！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                CheckStatus();
            }
            if (!runmgr.CenterStatus)
            {
                
                if (!runmgr.StartCenter())
                {
                    MessageBox.Show("启动Center出错！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                CheckStatus();
            }
        }


        private void FightButton_Click(object sender, EventArgs e)
        {
            CheckStatus();
            if (runmgr.FightStatus && MessageBox.Show("是否退出？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (!runmgr.StopFight())
                {
                    MessageBox.Show("结束Fight出错！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                CheckStatus();
            }
            if (!runmgr.FightStatus)
            {
                if (!runmgr.StartFight())
                {
                    MessageBox.Show("启动Fight出错！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                CheckStatus();
            }
        }

        private void RoadButton_Click(object sender, EventArgs e)
        {
            CheckStatus();
            if (runmgr.GameStatus && MessageBox.Show("是否退出？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (!runmgr.StopGame())
                {
                    MessageBox.Show("结束Game出错！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                CheckStatus();
            }
            if (!runmgr.GameStatus)
            {
 
                if (!runmgr.StartGame())
                {
                    MessageBox.Show("启动Game出错！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                CheckStatus();
            }
        }
        private void WebButton_Click(object sender, EventArgs e)
        {
            CheckStatus();
            if (runmgr.WebStatus && MessageBox.Show("是否退出？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (!runmgr.StopWeb())
                {
                    MessageBox.Show("结束WEB出错！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                CheckStatus();
            }
            if (!runmgr.WebStatus)
            {

                if (!runmgr.StartWeb())
                {
                    MessageBox.Show("启动WEB出错！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                CheckStatus();
            }
        }

        private void LauncherButton_Click(object sender, EventArgs e)
        {
            CheckStatus();
            if (runmgr.CenterStatus && runmgr.FightStatus && runmgr.GameStatus && runmgr.WebStatus && MessageBox.Show("是否终止全部？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (!runmgr.StopCenter())
                {
                    MessageBox.Show("结束Center出错！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (!runmgr.StopFight())
                {
                    MessageBox.Show("结束Fight出错！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (!runmgr.StopGame())
                {
                    MessageBox.Show("结束Game出错！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (!runmgr.StopWeb())
                {
                    MessageBox.Show("结束Web出错！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                CheckStatus();
            }
            else
            {
                if (!runmgr.CenterStatus)
                {
                    
                    if (!runmgr.StartCenter())
                    {
                        MessageBox.Show("启动Center出错！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    CheckStatus();
                    Thread.Sleep(1000);

                }
                if (!runmgr.FightStatus)
                {
                    if (!runmgr.StartFight())
                    {
                        MessageBox.Show("启动Fight出错！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    CheckStatus();
                    Thread.Sleep(1000);
                }
                if (!runmgr.GameStatus)
                {
                    if (!runmgr.StartGame())
                    {
                        MessageBox.Show("启动Game出错！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    CheckStatus();
                    Thread.Sleep(1000);
                }
                if (!runmgr.WebStatus)
                {
                    if (!runmgr.StartWeb())
                    {
                        MessageBox.Show("启动Web出错！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    CheckStatus();
                    Thread.Sleep(1000);
                }

            }
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!flag)
            {
                if (MessageBox.Show("你确定要退出?", "Exit", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                NotifyIcon.Dispose();
                runmgr.StopCenter();
                runmgr.StopFight();
                runmgr.StopGame();
                runmgr.StopWeb();
                runmgr.Dispose();
                flag = true;
            }
            System.Environment.Exit(0);
        }
        bool flag = false;

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }

        private void NotifyMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (this.Visible)
            {
                this.ToolStripMenuItem_Show_Hide.Text = "隐藏";
            }
            else
            {
                this.ToolStripMenuItem_Show_Hide.Text = "显示";
            }
        }

        private void ToolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            FormClosingEventArgs a = new FormClosingEventArgs(CloseReason.UserClosing, false);
            this.MainForm_FormClosing(sender,a);
        }

        private void ToolStripMenuItem_Show_Hide_Click(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }
    }
}