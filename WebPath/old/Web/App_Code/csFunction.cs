using SqlDataProvider.Data;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Text;
using Bussiness;
using zlib;

public class csFunction
{
    public static string ConvertSql(string inputString)
    {
        if (string.IsNullOrEmpty(inputString))
            return "";
        inputString = inputString.Trim().ToLower();
        inputString = inputString.Replace("'", "''");
        inputString = inputString.Replace(";--", "");
        inputString = inputString.Replace("=", "");
        inputString = inputString.Replace(" or", "");
        inputString = inputString.Replace(" or ", "");
        inputString = inputString.Replace(" and", "");
        inputString = inputString.Replace("and ", "");
        if (!csFunction.SqlChar(inputString))
        {
            inputString = "";
        }
        return inputString;

    }
    public static bool SqlChar(string v)
    {
        bool result;
        if (v.Trim() != "")
        {
            string[] array = csFunction.al;
            for (int i = 0; i < array.Length; i++)
            {
                string a = array[i];
                if (v.IndexOf(a + " ") > -1 || v.IndexOf(" " + a) > -1)
                {
                    result = false;
                    return result;
                }
            }
        }
        result = true;
        return result;
    }
    private static string[] al = ";|and|1=1|exec|insert|select|delete|update|like|count|chr|mid|master|or|truncate|char|declare|join".Split(new char[]
        {
            '|'
        });

    public static byte[] Compress(string str)
    {
        byte[] src = Encoding.UTF8.GetBytes(str);
        return csFunction.Compress(src);
    }
    public static byte[] Compress(byte[] src)
    {
        return csFunction.Compress(src, 0, src.Length);
    }
    public static byte[] Compress(byte[] src, int offset, int length)
    {
        MemoryStream ms = new MemoryStream();
        Stream s = new ZOutputStream(ms, 9);
        s.Write(src, offset, length);
        s.Close();
        return ms.ToArray();
    }
    public static string Uncompress(string str)
    {
        byte[] src = Encoding.UTF8.GetBytes(str);
        return Encoding.UTF8.GetString(csFunction.Uncompress(src));
    }
    public static byte[] Uncompress(byte[] src)
    {
        MemoryStream md = new MemoryStream();
        Stream d = new ZOutputStream(md);
        d.Write(src, 0, src.Length);
        d.Close();
        return md.ToArray();
    }


}