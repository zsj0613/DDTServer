using Lsj.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DDTTools
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.FixedSize();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "请选择文件";
            openFileDialog.Filter = "所有文件(*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream fileStream = File.OpenRead(openFileDialog.FileName);
                byte[] array = new byte[fileStream.Length];
                fileStream.Read(array, 0, (int)fileStream.Length);
                fileStream.Close();
                byte[] buffer = new byte[21];
                new Random().NextBytes(buffer);
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.AddExtension = true;
                saveFileDialog.Filter = "所有文件(*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileStream = File.Create(saveFileDialog.FileName);
                    fileStream.Write(buffer, 0, 21);
                    fileStream.Write(array, 124, array.Length - 124);
                    fileStream.Write(array, 3, 121);
                    fileStream.Flush();
                    fileStream.Close();
                    WinForm.Notice("Encrypted");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "请选择文件";
            openFileDialog.Filter = "所有文件(*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream fileStream = File.OpenRead(openFileDialog.FileName);
                byte[] array = new byte[fileStream.Length];
                fileStream.Read(array, 0, (int)fileStream.Length);
                fileStream.Close();
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.AddExtension = true;
                saveFileDialog.Filter = "SWF文件(*.swf)|*.swf";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileStream = File.Create(saveFileDialog.FileName);
                    fileStream.WriteByte(67);
                    fileStream.WriteByte(87);
                    fileStream.WriteByte(83);
                    Stream arg_AD_0 = fileStream;
                    byte[] expr_A5 = array;
                    arg_AD_0.Write(expr_A5, expr_A5.Length - 121, 121);
                    fileStream.Write(array, 21, array.Length - 121);
                    fileStream.Flush();
                    fileStream.Close();
                    WinForm.Notice("Descrypted");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog.SelectedPath;
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.AddExtension = true;
                saveFileDialog.Filter = "DLL文件(*.dll)|*.dll";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = saveFileDialog.FileName;
                    CsComplier csComplier = new CsComplier();
                    csComplier.path = selectedPath;
                    csComplier.target = fileName;
                    csComplier.@using = new string[]
                    {
                        "System.Drawing.dll",
                        "GameLibrary.dll",
                        "Bussiness.dll",
                        "SqlDataProvider.dll",
                        "Game.Base.dll"
                    };
                    string text = "";
                    if (csComplier.Complie(ref text))
                    {
                        WinForm.Notice("Complied");
                    }
                    else
                    {
                        WinForm.Notice("Failed");
                    }
                    this.textBox1.AppendText(text);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog.SelectedPath;
                FolderBrowserDialog folderBrowserDialog2 = new FolderBrowserDialog();
                if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath2 = folderBrowserDialog2.SelectedPath;
                    string[] directories = Directory.GetDirectories(selectedPath);
                    if (directories.Length != 0 && !string.IsNullOrEmpty(selectedPath2))
                    {
                        if (!Directory.Exists(selectedPath2))
                        {
                            Directory.CreateDirectory(selectedPath2);
                        }
                        this.textBox1.AppendText(string.Format("共 {0} 文件...\r\n", directories.Length));
                        string[] array = directories;
                        for (int i = 0; i < array.Length; i++)
                        {
                            string arg = array[i];
                            FileInfo fileInfo = new FileInfo(string.Format("{0}\\{1}", arg, "fore.png"));
                            if (File.Exists(fileInfo.FullName))
                            {
                                Tile tile = new Tile(new Bitmap(fileInfo.FullName), true);
                                FileInfo fileInfo2 = new FileInfo(string.Format("{0}\\{1}\\{2}", selectedPath2, fileInfo.Directory.Name, "fore.map"));
                                this.textBox1.AppendText(string.Format("开始生成 地图 {0}------>{1}\r\n", fileInfo.FullName, fileInfo2.FullName));
                                if (!Directory.Exists(fileInfo2.DirectoryName))
                                {
                                    Directory.CreateDirectory(fileInfo2.DirectoryName);
                                }
                                FileStream expr_137 = File.Create(fileInfo2.FullName);
                                BinaryWriter expr_13D = new BinaryWriter(expr_137);
                                expr_13D.Write(tile.Width);
                                expr_13D.Write(tile.Height);
                                expr_13D.Flush();
                                expr_137.Write(tile.Data, 0, tile.Data.Length);
                                expr_137.Close();
                            }
                            fileInfo = new FileInfo(string.Format("{0}\\{1}", arg, "dead.png"));
                            if (File.Exists(fileInfo.FullName))
                            {
                                Tile tile2 = new Tile(new Bitmap(fileInfo.FullName), true);
                                FileInfo fileInfo3 = new FileInfo(string.Format("{0}\\{1}\\{2}", selectedPath2, fileInfo.Directory.Name, "dead.map"));
                                if (!Directory.Exists(fileInfo3.DirectoryName))
                                {
                                    Directory.CreateDirectory(fileInfo3.DirectoryName);
                                }
                                this.textBox1.AppendText(string.Format("开始生成 地图 {0}------>{1}\r\n", fileInfo.FullName, fileInfo3.FullName));
                                FileStream expr_222 = File.Create(fileInfo3.FullName);
                                BinaryWriter expr_228 = new BinaryWriter(expr_222);
                                expr_228.Write(tile2.Width);
                                expr_228.Write(tile2.Height);
                                expr_228.Flush();
                                expr_222.Write(tile2.Data, 0, tile2.Data.Length);
                                expr_222.Close();
                            }
                        }
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "请选择文件";
            openFileDialog.Filter = "所有文件(*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream fileStream = File.OpenRead(openFileDialog.FileName);
                byte[] array = new byte[fileStream.Length];
                fileStream.Read(array, 0, (int)fileStream.Length);
                fileStream.Close();
                byte[] buffer = new byte[100];
                new Random().NextBytes(buffer);
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.AddExtension = true;
                saveFileDialog.Filter = "所有文件(*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileStream = File.Create(saveFileDialog.FileName);
                    fileStream.Write(buffer, 0, 100);
                    fileStream.Write(array, 200, array.Length - 200);
                    fileStream.Write(array, 3, 197);
                    fileStream.Flush();
                    fileStream.Close();
                    WinForm.Notice("Encrypted");
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "请选择文件";
            openFileDialog.Filter = "所有文件(*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream fileStream = File.OpenRead(openFileDialog.FileName);
                byte[] array = new byte[fileStream.Length];
                fileStream.Read(array, 0, (int)fileStream.Length);
                fileStream.Close();
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.AddExtension = true;
                saveFileDialog.Filter = "SWF文件(*.swf)|*.swf";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileStream = File.Create(saveFileDialog.FileName);
                    fileStream.WriteByte(67);
                    fileStream.WriteByte(87);
                    fileStream.WriteByte(83);
                    Stream arg_B3_0 = fileStream;
                    byte[] expr_A5 = array;
                    arg_B3_0.Write(expr_A5, expr_A5.Length - 197, 197);
                    fileStream.Write(array, 100, array.Length - 297);
                    fileStream.Flush();
                    fileStream.Close();
                    WinForm.Notice("Descrypted");
                }
            }
        }
    }
}
