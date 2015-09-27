using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using log4net;
using System.Reflection;
using System.Configuration;

namespace Game.Language
{
    public class LanguageMgr
    {
        private static bool m_Loaded = false;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static Hashtable LangsSentences = new Hashtable();
        public static bool Load()
        {
            bool result;
            {
                m_Loaded = true;
                Hashtable temp = LanguageMgr.LoadLanguage();
                if (temp.Count > 0)
                {
                    Interlocked.Exchange<Hashtable>(ref LanguageMgr.LangsSentences, temp);
                    result = true;
                    return result;
                }
            }
            result = false;
            return result;
        }
        private static Hashtable LoadLanguage()
        {
            Hashtable list = new Hashtable();
            try
            {



                string[] lines = Language.language;
                IList textList = new ArrayList(lines);
                foreach (string line in textList)
                {
                    if (line.IndexOf(':') != -1)
                    {
                        string[] splitted = new string[]
                        {
                                line.Substring(0, line.IndexOf(':')),
                                line.Substring(line.IndexOf(':') + 1)
                        };
                        splitted[1] = splitted[1].Replace("\t", "");
                        list[splitted[0]] = splitted[1];
                    }
                }

                string a = @".\Languages";
                if (ConfigurationManager.AppSettings["LanguagePath"] != null)
                {
                    a = ConfigurationManager.AppSettings["LanguagePath"];
                }
                //Load from txt
                if (System.IO.Directory.Exists(a))
                {
                    string[] files = System.IO.Directory.GetFiles(a+@"\", "language*.txt");
                    foreach (string file in files)
                    {
                        log.Debug(@"Load Languages from "+file);
                        string[] lines2 = System.IO.File.ReadAllLines(file);
                        IList textList2 = new ArrayList(lines2);
                        foreach (string line in textList2)
                        {
                            if (line.IndexOf(':') != -1)
                            {
                                string[] splitted = new string[]
                                {
                                line.Substring(0, line.IndexOf(':')),
                                line.Substring(line.IndexOf(':') + 1)
                                };
                                splitted[1] = splitted[1].Replace("\t", "");
                                list[splitted[0]] = splitted[1];
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                LanguageMgr.log.Error("LanguageMgr",e);
            }

            return list;
        }
        public static string GetTranslation(string translateId, params object[] args)
        {
            if (!m_Loaded)
            {
                Load();
            }

            string result = "";
            try
            {
                

                if (LanguageMgr.LangsSentences.ContainsKey(translateId))
                {
                    string translated = (string)LanguageMgr.LangsSentences[translateId];
                    {
                        translated = string.Format(translated, args);
                    }

                    result = ((translated == null) ? translateId : translated);
                }
                else
                {
                    result = translateId;
                }
            }

            catch (Exception e)
            {
                LanguageMgr.log.Error("LanguageMgr", e);
            }

            return result;
        }
    }
    
}
