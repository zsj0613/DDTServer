using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;

/// <summary>
/// FileSystem 的摘要说明
/// </summary>

    public class FileSystem
    {
        public ArrayList contentList = new ArrayList();
        private FileSystemWatcher fileWatcher = new FileSystemWatcher();
        private string filePath = string.Empty;
        private string fileDirectory = string.Empty;
        private string fileType = string.Empty;
        private static string illegalDirectory = HttpContext.Current.Server.MapPath("/illegal/");
        public FileSystem()
        {
            string[] directoryEntries = Directory.GetFileSystemEntries(FileSystem.illegalDirectory, "illegal.txt");
            if (this.contentList.Count > 0)
            {
                this.contentList.Clear();
            }
            string[] array = directoryEntries;
            for (int i = 0; i < array.Length; i++)
            {
                string path = array[i];
                this.initContent(path);
            }
        }
        private void initContent(string Path)
        {
            if (File.Exists(Path))
            {
                this.filePath = Path;
                StreamReader sr = new StreamReader(Path, Encoding.GetEncoding("GB2312"));
                string str = "";
                while (str != null)
                {
                    str = sr.ReadLine();
                    if (!string.IsNullOrEmpty(str))
                    {
                        this.contentList.Add(str);
                    }
                }
                if (str == null)
                {
                    sr.Close();
                }
            }
        }
        public bool checkIllegalChar(string strRegName)
        {
            bool flag = false;
            if (!string.IsNullOrEmpty(strRegName))
            {
                flag = this.checkChar(strRegName);
            }
            return flag;
        }
        private bool checkChar(string strRegName)
        {
            bool flag = false;
            foreach (string strLine in this.contentList)
            {
                if (!strLine.StartsWith("GM"))
                {
                    string text = strLine;
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (strRegName.Contains(text[i].ToString()))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
                else
                {
                    string[] keyword = strLine.Split(new char[]
                    {
                        '|'
                    });
                    string[] array = keyword;
                    for (int i = 0; i < array.Length; i++)
                    {
                        string key = array[i];
                        if (strRegName.Contains(key) && key != "")
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }
            return flag;
        }
    }
