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
            this.LauncherButton = new System.Windows.Forms.Button();
            this.CenterButton = new System.Windows.Forms.Button();
            this.FightButton = new System.Windows.Forms.Button();
            this.RoadButton = new System.Windows.Forms.Button();
            this.IsSlient = new System.Windows.Forms.CheckBox();
            this.WebButton = new System.Windows.Forms.Button();
            this.TabControlMain = new System.Windows.Forms.TabControl();
            this.NotifyMenuStrip.SuspendLayout();
            this.TabLauncher.SuspendLayout();
            this.TabControlMain.SuspendLayout();
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
            this.NotifyMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Show_Hide,
            this.ToolStripMenuItem_Exit});
            this.NotifyMenuStrip.Name = "NotifyMenuStrip";
            this.NotifyMenuStrip.ShowImageMargin = false;
            this.NotifyMenuStrip.Size = new System.Drawing.Size(208, 48);
            this.NotifyMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.NotifyMenuStrip_Opening);
            // 
            // ToolStripMenuItem_Show_Hide
            // 
            this.ToolStripMenuItem_Show_Hide.Name = "ToolStripMenuItem_Show_Hide";
            this.ToolStripMenuItem_Show_Hide.Size = new System.Drawing.Size(207, 22);
            this.ToolStripMenuItem_Show_Hide.Text = "ToolStripMenuItem_Show_Hide";
            this.ToolStripMenuItem_Show_Hide.Click += new System.EventHandler(this.ToolStripMenuItem_Show_Hide_Click);
            // 
            // ToolStripMenuItem_Exit
            // 
            this.ToolStripMenuItem_Exit.Name = "ToolStripMenuItem_Exit";
            this.ToolStripMenuItem_Exit.Size = new System.Drawing.Size(207, 22);
            this.ToolStripMenuItem_Exit.Text = "退出程序";
            this.ToolStripMenuItem_Exit.Click += new System.EventHandler(this.ToolStripMenuItem_Exit_Click);
            // 
            // TabLauncher
            // 
            this.TabLauncher.Controls.Add(this.WebButton);
            this.TabLauncher.Controls.Add(this.IsSlient);
            this.TabLauncher.Controls.Add(this.RoadButton);
            this.TabLauncher.Controls.Add(this.FightButton);
            this.TabLauncher.Controls.Add(this.CenterButton);
            this.TabLauncher.Controls.Add(this.LauncherButton);
            this.TabLauncher.Location = new System.Drawing.Point(4, 22);
            this.TabLauncher.Name = "TabLauncher";
            this.TabLauncher.Padding = new System.Windows.Forms.Padding(3);
            this.TabLauncher.Size = new System.Drawing.Size(874, 332);
            this.TabLauncher.TabIndex = 0;
            this.TabLauncher.Text = "启动";
            this.TabLauncher.UseVisualStyleBackColor = true;
            // 
            // LauncherButton
            // 
            this.LauncherButton.Font = new System.Drawing.Font("微软雅黑", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LauncherButton.Location = new System.Drawing.Point(75, 63);
            this.LauncherButton.Name = "LauncherButton";
            this.LauncherButton.Size = new System.Drawing.Size(221, 177);
            this.LauncherButton.TabIndex = 1;
            this.LauncherButton.Text = "LauncherButton";
            this.LauncherButton.UseVisualStyleBackColor = true;
            this.LauncherButton.Click += new System.EventHandler(this.LauncherButton_Click);
            // 
            // CenterButton
            // 
            this.CenterButton.Location = new System.Drawing.Point(380, 82);
            this.CenterButton.Name = "CenterButton";
            this.CenterButton.Size = new System.Drawing.Size(192, 40);
            this.CenterButton.TabIndex = 2;
            this.CenterButton.Text = "CenterButton";
            this.CenterButton.UseVisualStyleBackColor = true;
            this.CenterButton.Click += new System.EventHandler(this.CenterButton_Click);
            // 
            // FightButton
            // 
            this.FightButton.Location = new System.Drawing.Point(636, 82);
            this.FightButton.Name = "FightButton";
            this.FightButton.Size = new System.Drawing.Size(192, 40);
            this.FightButton.TabIndex = 3;
            this.FightButton.Text = "FightButton";
            this.FightButton.UseVisualStyleBackColor = true;
            this.FightButton.Click += new System.EventHandler(this.FightButton_Click);
            // 
            // RoadButton
            // 
            this.RoadButton.Location = new System.Drawing.Point(380, 183);
            this.RoadButton.Name = "RoadButton";
            this.RoadButton.Size = new System.Drawing.Size(192, 40);
            this.RoadButton.TabIndex = 4;
            this.RoadButton.Text = "RoadButton";
            this.RoadButton.UseVisualStyleBackColor = true;
            this.RoadButton.Click += new System.EventHandler(this.RoadButton_Click);
            // 
            // IsSlient
            // 
            this.IsSlient.AutoSize = true;
            this.IsSlient.Location = new System.Drawing.Point(112, 276);
            this.IsSlient.Name = "IsSlient";
            this.IsSlient.Size = new System.Drawing.Size(72, 16);
            this.IsSlient.TabIndex = 5;
            this.IsSlient.Text = "安静模式";
            this.IsSlient.UseVisualStyleBackColor = true;
            this.IsSlient.CheckedChanged += new System.EventHandler(this.IsSlient_CheckedChanged);
            // 
            // WebButton
            // 
            this.WebButton.Location = new System.Drawing.Point(636, 183);
            this.WebButton.Name = "WebButton";
            this.WebButton.Size = new System.Drawing.Size(192, 40);
            this.WebButton.TabIndex = 8;
            this.WebButton.Text = "WebButton";
            this.WebButton.UseVisualStyleBackColor = true;
            this.WebButton.Click += new System.EventHandler(this.WebButton_Click);
            // 
            // TabControlMain
            // 
            this.TabControlMain.Controls.Add(this.TabLauncher);
            this.TabControlMain.Location = new System.Drawing.Point(2, 1);
            this.TabControlMain.Name = "TabControlMain";
            this.TabControlMain.SelectedIndex = 0;
            this.TabControlMain.Size = new System.Drawing.Size(882, 358);
            this.TabControlMain.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 355);
            this.Controls.Add(this.TabControlMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "DDTank Launcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.NotifyMenuStrip.ResumeLayout(false);
            this.TabLauncher.ResumeLayout(false);
            this.TabLauncher.PerformLayout();
            this.TabControlMain.ResumeLayout(false);
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
        private System.Windows.Forms.CheckBox IsSlient;
        private System.Windows.Forms.Button RoadButton;
        private System.Windows.Forms.Button FightButton;
        private System.Windows.Forms.Button CenterButton;
        private System.Windows.Forms.Button LauncherButton;
        private System.Windows.Forms.TabControl TabControlMain;
    }
}

