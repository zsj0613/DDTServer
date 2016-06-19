namespace Game.Launcher
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.StatusTimer = new System.Windows.Forms.Timer(this.components);
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.NotifyMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItem_Show_Hide = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.TabLauncher = new System.Windows.Forms.TabPage();
            this.WebButton = new System.Windows.Forms.Button();
            this.RoadButton = new System.Windows.Forms.Button();
            this.FightButton = new System.Windows.Forms.Button();
            this.CenterButton = new System.Windows.Forms.Button();
            this.LauncherButton = new System.Windows.Forms.Button();
            this.TabControlMain = new System.Windows.Forms.TabControl();
            this.TabLog = new System.Windows.Forms.TabPage();
            this.LogView = Loading.LogView;

            this.NotifyMenuStrip.SuspendLayout();
            this.TabLauncher.SuspendLayout();
            this.TabControlMain.SuspendLayout();
            this.TabLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusTimer
            // 
            this.StatusTimer.Enabled = true;
            this.StatusTimer.Interval = 1000;
            this.StatusTimer.Tick += new System.EventHandler(this.StatusTimer_Tick);
            // 
            // NotifyIcon
            // 
            this.NotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("NotifyIcon.Icon")));
            this.NotifyIcon.Text = "DDTank Launcher";
            this.NotifyIcon.Visible = true;
            this.NotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_MouseDoubleClick);
            // 
            // NotifyMenuStrip
            // 
            this.NotifyMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.NotifyMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Show_Hide,
            this.ToolStripMenuItem_Exit});
            this.NotifyMenuStrip.Name = "NotifyMenuStrip";
            this.NotifyMenuStrip.ShowImageMargin = false;
            this.NotifyMenuStrip.Size = new System.Drawing.Size(286, 56);
            this.NotifyMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.NotifyMenuStrip_Opening);
            // 
            // ToolStripMenuItem_Show_Hide
            // 
            this.ToolStripMenuItem_Show_Hide.Name = "ToolStripMenuItem_Show_Hide";
            this.ToolStripMenuItem_Show_Hide.Size = new System.Drawing.Size(285, 26);
            this.ToolStripMenuItem_Show_Hide.Text = "ToolStripMenuItem_Show_Hide";
            this.ToolStripMenuItem_Show_Hide.Click += new System.EventHandler(this.ToolStripMenuItem_Show_Hide_Click);
            // 
            // ToolStripMenuItem_Exit
            // 
            this.ToolStripMenuItem_Exit.Name = "ToolStripMenuItem_Exit";
            this.ToolStripMenuItem_Exit.Size = new System.Drawing.Size(285, 26);
            this.ToolStripMenuItem_Exit.Text = "退出程序";
            this.ToolStripMenuItem_Exit.Click += new System.EventHandler(this.ToolStripMenuItem_Exit_Click);
            // 
            // TabLauncher
            // 
            this.TabLauncher.Controls.Add(this.WebButton);
            this.TabLauncher.Controls.Add(this.RoadButton);
            this.TabLauncher.Controls.Add(this.FightButton);
            this.TabLauncher.Controls.Add(this.CenterButton);
            this.TabLauncher.Controls.Add(this.LauncherButton);
            this.TabLauncher.Location = new System.Drawing.Point(4, 25);
            this.TabLauncher.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TabLauncher.Name = "TabLauncher";
            this.TabLauncher.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TabLauncher.Size = new System.Drawing.Size(1168, 419);
            this.TabLauncher.TabIndex = 0;
            this.TabLauncher.Text = "启动";
            this.TabLauncher.UseVisualStyleBackColor = true;
            // 
            // WebButton
            // 
            this.WebButton.Location = new System.Drawing.Point(848, 229);
            this.WebButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.WebButton.Name = "WebButton";
            this.WebButton.Size = new System.Drawing.Size(256, 50);
            this.WebButton.TabIndex = 8;
            this.WebButton.Text = "WebButton";
            this.WebButton.UseVisualStyleBackColor = true;
            this.WebButton.Click += new System.EventHandler(this.WebButton_Click);
            // 
            // RoadButton
            // 
            this.RoadButton.Location = new System.Drawing.Point(507, 229);
            this.RoadButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.RoadButton.Name = "RoadButton";
            this.RoadButton.Size = new System.Drawing.Size(256, 50);
            this.RoadButton.TabIndex = 4;
            this.RoadButton.Text = "RoadButton";
            this.RoadButton.UseVisualStyleBackColor = true;
            this.RoadButton.Click += new System.EventHandler(this.RoadButton_Click);
            // 
            // FightButton
            // 
            this.FightButton.Location = new System.Drawing.Point(848, 102);
            this.FightButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.FightButton.Name = "FightButton";
            this.FightButton.Size = new System.Drawing.Size(256, 50);
            this.FightButton.TabIndex = 3;
            this.FightButton.Text = "FightButton";
            this.FightButton.UseVisualStyleBackColor = true;
            this.FightButton.Click += new System.EventHandler(this.FightButton_Click);
            // 
            // CenterButton
            // 
            this.CenterButton.Location = new System.Drawing.Point(507, 102);
            this.CenterButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CenterButton.Name = "CenterButton";
            this.CenterButton.Size = new System.Drawing.Size(256, 50);
            this.CenterButton.TabIndex = 2;
            this.CenterButton.Text = "CenterButton";
            this.CenterButton.UseVisualStyleBackColor = true;
            this.CenterButton.Click += new System.EventHandler(this.CenterButton_Click);
            // 
            // LauncherButton
            // 
            this.LauncherButton.Font = new System.Drawing.Font("微软雅黑", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LauncherButton.Location = new System.Drawing.Point(100, 79);
            this.LauncherButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.LauncherButton.Name = "LauncherButton";
            this.LauncherButton.Size = new System.Drawing.Size(295, 221);
            this.LauncherButton.TabIndex = 1;
            this.LauncherButton.Text = "LauncherButton";
            this.LauncherButton.UseVisualStyleBackColor = true;
            this.LauncherButton.Click += new System.EventHandler(this.LauncherButton_Click);
            // 
            // TabControlMain
            // 
            this.TabControlMain.Controls.Add(this.TabLauncher);
            this.TabControlMain.Controls.Add(this.TabLog);
            this.TabControlMain.Location = new System.Drawing.Point(3, 1);
            this.TabControlMain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TabControlMain.Name = "TabControlMain";
            this.TabControlMain.SelectedIndex = 0;
            this.TabControlMain.Size = new System.Drawing.Size(1176, 448);
            this.TabControlMain.TabIndex = 0;
            // 
            // TabLog
            // 
            this.TabLog.Controls.Add(this.LogView);
            this.TabLog.Location = new System.Drawing.Point(4, 25);
            this.TabLog.Name = "TabLog";
            this.TabLog.Padding = new System.Windows.Forms.Padding(3);
            this.TabLog.Size = new System.Drawing.Size(1168, 419);
            this.TabLog.TabIndex = 1;
            this.TabLog.Text = "日志";
            this.TabLog.UseVisualStyleBackColor = true;
            // 
            // LogView
            // 
            this.LogView.Location = new System.Drawing.Point(6, 6);
            this.LogView.Name = "LogView";
            this.LogView.Size = new System.Drawing.Size(1152, 400);
            this.LogView.TabIndex = 0;
            this.LogView.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1177, 444);
            this.Controls.Add(this.TabControlMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "DDTank Launcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.NotifyMenuStrip.ResumeLayout(false);
            this.TabLauncher.ResumeLayout(false);
            this.TabControlMain.ResumeLayout(false);
            this.TabLog.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer StatusTimer;
        private System.Windows.Forms.NotifyIcon NotifyIcon;
        private System.Windows.Forms.ContextMenuStrip NotifyMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Show_Hide;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Exit;
        private System.Windows.Forms.TabPage TabLauncher;
        private System.Windows.Forms.Button WebButton;
        private System.Windows.Forms.Button RoadButton;
        private System.Windows.Forms.Button FightButton;
        private System.Windows.Forms.Button CenterButton;
        private System.Windows.Forms.Button LauncherButton;
        private System.Windows.Forms.TabControl TabControlMain;
        private System.Windows.Forms.TabPage TabLog;
        private System.Windows.Forms.RichTextBox LogView;

    }
}

