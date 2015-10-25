using Bussiness;
using Lsj.Util;
using Lsj.Util.Net.Web;
using Lsj.Util.Net.Web.Response;
using NVelocityTemplateEngine;
using NVelocityTemplateEngine.Interfaces;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Web.Server.Module
{
    public static class UserList
    {
        public static void ProcessUserList(ref HttpResponse response, HttpRequest request)
        {
            string search = request.Form["Tb_SearchKeys"];
            int page = request.QueryString["pages"].ConvertToInt(1);
            INVelocityEngine FileEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(Server.ModulePath + @"vm", false);

            IDictionary context = new Hashtable();

            List<UserInfo> result = new ManageBussiness().GetAllUserInfo();
            result.Sort(CompareByID);
            if (search != "")
            {
                result = result.FindAll(
                    delegate (UserInfo a)
                    {
                        return a.UserName.IndexOf(search, 0) != -1 || a.NickName.IndexOf(search, 0) != -1;
                    }
                    );
            }
            int count = result.Count;
            int tocalpage = 1;
            if (search == "")
            {
                tocalpage = Convert.ToInt32(Math.Ceiling((decimal)count / 20));
                result = result.Skip((page - 1) * 20).Take(20).ToList();
            }
            context.Add("Result", result);
            context.Add("Search", search);
            context.Add("Count", count);
            context.Add("Page", page);
            context.Add("TocalPage", tocalpage);

            var x = FileEngine.Process(context, "UserList.vm");
            response.Write(x);
        }
        public static int CompareByID(UserInfo x, UserInfo y)//从小到大排序器  
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }

                return 1;

            }
            if (y == null)
            {
                return -1;
            }
            int retval = -(y.UserID.CompareTo(x.UserID));
            return retval;
        }
    }
}
