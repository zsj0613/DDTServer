using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Bussiness;
using Center.Server;
using Lsj.Util;
using Lsj.Util.Logs;
using Lsj.Util.Net.Web.Interfaces;
using Lsj.Util.Net.Web.Post;
using Lsj.Util.Text;
using NVelocityTemplateEngine;
using NVelocityTemplateEngine.Interfaces;
using SqlDataProvider.Data;

namespace Web.Server.Modules.Admin
{
    public class AdminPage
    {

        public static void Process(IHttpRequest Request, IHttpResponse Response)
        {
            string username = Request.Cookies["user"].content;
            string password = Request.Cookies["pass"].content;
            int usertype = 0;


            using (var a = new MemberShipBussiness())
            {
                var result = a.CheckUser(username, password);
                if (result)
                {
                    using (var b = new PlayerBussiness())
                    {
                        usertype = b.GetUserType(username);
                    }
                }
            }
            if (usertype >= 2)
            {
                var page = Request.Uri.QueryString["page"].ToSafeString();
                switch (page)
                {
                    case "left":
                        ProcessLeft(Request, Response);
                        break;
                    case "userlist":
                        ProcessUserlist(Request, Response);
                        break;
                    case "version":
                        ProcessVersion(Request, Response);
                        break;
                    case "top":
                        ProcessTop(Request, Response,username);
                        break;
                    case "mail":
                        ProcessMail(Request, Response, usertype);
                        break;
                    case "notice":
                        ProcessNotice(Request, Response);
                        break;
                    case "status":
                        ProcessStatus(Request, Response);
                        break;
                    default:
                        ProcessIndex(Request, Response);
                        break;
                }
            }
            else
            {
                Response.Write("你无权访问GM管理平台！");
            }
        }

        private static void ProcessUserlist(IHttpRequest Request, IHttpResponse Response)
        {
            var postdata = Request.Content.ReadAll().ConvertFromBytes(Encoding.UTF8);
            var Form = FormParser.Parse(postdata);
            string search = Form["Tb_SearchKeys"].ToSafeString();
            int num = StringHelper.ConvertToInt(Request.Uri.QueryString["pages"], 1);
            INVelocityEngine iNVelocityEngine = NVelocityEngineFactory.CreateNVelocityAssemblyEngine("Web.Server", false);
            IDictionary dictionary = new Hashtable();
            using (var x = new ManageBussiness())
            {
                List<UserInfo> list = x.GetAllUserInfo().ToList();
                var onlinecount = list.Where((a) => (a.State != 0)).Count();
                list.Sort(new Comparison<UserInfo>(CompareByID));
                if (search != "")
                {
                    list = list.FindAll((UserInfo a) => a.UserName.IndexOf(search, 0) != -1 || a.NickName.IndexOf(search, 0) != -1);
                }
                int count = list.Count;
                int num2 = 1;
                if (search == "")
                {
                    num2 = Convert.ToInt32(Math.Ceiling(count / 20m));
                    list = list.Skip((num - 1) * 20).Take(20).ToList<UserInfo>();
                }
                dictionary.Add("loginuri", $"http://{WebServer.Instance.Config.GameDomain}/game.aspx");
                dictionary.Add("Result", list);
                dictionary.Add("OnlineCount", onlinecount);
                dictionary.Add("Search", search);
                dictionary.Add("Count", count);
                dictionary.Add("Page", num);
                dictionary.Add("TocalPage", num2);
                string text = iNVelocityEngine.Process(dictionary, "Web.Server.Modules.Admin.UserList.vm");
                Response.Write(text);
            }
        }

        private static void ProcessVersion(IHttpRequest Request, IHttpResponse Response)
        {
            INVelocityEngine iNVelocityEngine = NVelocityEngineFactory.CreateNVelocityAssemblyEngine("Web.Server", false);
            IDictionary context = new Hashtable();
            Response.Write(iNVelocityEngine.Process(context, "Web.Server.Modules.Admin.Admin_Version.vm"));
        }

        private static void ProcessTop(IHttpRequest Request, IHttpResponse Response,string username)
        {
            INVelocityEngine iNVelocityEngine = NVelocityEngineFactory.CreateNVelocityAssemblyEngine("Web.Server", false);
            IDictionary dictionary = new Hashtable();
            dictionary.Add("UserName", username);
            Response.Write(iNVelocityEngine.Process(dictionary, "Web.Server.Modules.Admin.Admin_Top.vm"));
        }

        private static void ProcessStatus(IHttpRequest Request, IHttpResponse Response)
        {
            INVelocityEngine iNVelocityEngine = NVelocityEngineFactory.CreateNVelocityAssemblyEngine("Web.Server", false);
            IDictionary dictionary = new Hashtable();
            var x = WebServer.Runmgr;
            var a = new bool[] { x.CenterStatus, x.FightStatus, x.GameStatus, };
            dictionary.Add("Runmgr", a);
            dictionary.Add("IsConnected", WebServer.Instance.IsOpen);
            Response.Write(iNVelocityEngine.Process(dictionary, "Web.Server.Modules.Admin.Status.vm"));
        }

        private static void ProcessLeft(IHttpRequest Request, IHttpResponse Response)
        {
            INVelocityEngine iNVelocityEngine = NVelocityEngineFactory.CreateNVelocityAssemblyEngine("Web.Server", false);
            IDictionary context = new Hashtable();
            context.Add("loginuri", $"http://{WebServer.Instance.Config.GameDomain}/game.aspx");
            Response.Write(iNVelocityEngine.Process(context, "Web.Server.Modules.Admin.Admin_Left.vm"));
        }

        private static DataTable tabGoods;
        private static bool SendMail(IDictionary<string, string> Form)
        {
            bool result = false;
            var Title = System.Web.HttpUtility.UrlDecode(Form["txt_Title"].ToSafeString());
            var Gold = Form["txt_gold"].ConvertToInt();
            var Money = Form["txt_money"].ConvertToInt();
            var GiftToken = Form["txt_liJuan"].ConvertToInt();
            var Content = System.Web.HttpUtility.UrlDecode(Form["txt_Content"].ToSafeString());
            var str = "";
            if (tabGoods != null)
            {
                foreach (DataRow x in tabGoods.Rows)
                {
                    str += string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}|", new object[]
                        {
                        x["TemplateId"],
                        x["GoodNumber"],
                        x["ValidDate"],
                        x["StrengthenLevel"],
                        x["AttackCompose"],
                        x["DefendCompose"],
                        x["AgilityCompose"],
                        x["LuckCompose"],
                        (x["IsBind"].ToString() == "True") ? 1 : 0
                        });
                }
                str.RemoveLastOne();
            }
            int[] IDs = Form["txt_userID"].Split(',').ConvertToIntArray();
            using (var a = new PlayerBussiness())
            {
                foreach (var id in IDs)
                {
                    a.SendMailAndItem(Title, Content, id, Gold, Money, GiftToken, str);
                    OpenAPIs.MailNotice(id);
                }
            }
            result = true;
            tabGoods = null;
            return result;

        }

        private static void ProcessMail(IHttpRequest Request, IHttpResponse Response, int usertype)
        {
            if (!(usertype > 2))
            {
                Response.Write("对不起,你的权限不足");
                return;
            }
            using (var xx = new ManageBussiness())
            {
                using (var xy = new ProduceBussiness())
                {
                    var postdata = Request.Content.ReadAll().ConvertFromBytes(Encoding.UTF8);
                    var Form = FormParser.Parse(postdata);

                    if (Request.Uri.QueryString["sub_searchUserId"].ToSafeString() != "")
                    {
                        string userMsg = Request.Uri.QueryString["userMsg"].ToSafeString();
                        string getAllUserId = "";
                        if (userMsg != "")
                        {
                            string[] strUsersg = userMsg.Split(new char[]
                            {
                        ','
                            });
                            List<UserInfo> result = xx.GetAllUserInfo().ToList();
                            for (int i = 0; i < strUsersg.Count<string>(); i++)
                            {
                                foreach (var a in result.FindAll((t) =>
                                {
                                    if (t.UserName.IndexOf(strUsersg[i]) != -1 || t.NickName.IndexOf(strUsersg[i]) != -1)
                                        return true;

                                    else
                                        return false;

                                }))
                                {
                                    getAllUserId += string.Concat(new string[]
                                        {
                                    "ID:[",
                                    a.UserID.ToString(),
                                    "],用户名:[",
                                    a.UserName.ToString(),
                                    "],昵称:[",
                                    a.NickName.ToString(),
                                    "]\n"
                                        });
                                }

                            }
                            Response.Write(getAllUserId);
                            return;
                        }
                    }
                    else if (Form["txt_userID"].ToSafeString() != "" && Form["txt_Title"].ToSafeString() != "" && Form["txt_Content"].ToSafeString() != "")
                    {
                        if (SendMail(Form))
                        {
                            Response.Write("成功");
                        }
                        else
                        {
                            Response.Write("失败");
                        }
                    }
                    else
                    {
                        INVelocityEngine iNVelocityEngine = NVelocityEngineFactory.CreateNVelocityAssemblyEngine("Web.Server", false);
                        IDictionary context = new Hashtable();
                        if (Form["btn_wq"].ToSafeString() != "")
                        {
                            List<ItemTemplateInfo> GoodsWeapons = xy.GetSingleCategory(7).ToList();
                            context.Add("GoodsWeapons", GoodsWeapons);
                        }
                        if (Form["btn_zb"].ToSafeString() != "")
                        {
                            List<ItemTemplateInfo> GoodsEquipment = xy.GetSingleCategory(1).ToList();
                            GoodsEquipment.AddRange(xy.GetSingleCategory(2).ToList());
                            GoodsEquipment.AddRange(xy.GetSingleCategory(3).ToList());
                            GoodsEquipment.AddRange(xy.GetSingleCategory(4).ToList());
                            GoodsEquipment.AddRange(xy.GetSingleCategory(5).ToList());
                            GoodsEquipment.AddRange(xy.GetSingleCategory(6).ToList());
                            GoodsEquipment.AddRange(xy.GetSingleCategory(8).ToList());
                            GoodsEquipment.AddRange(xy.GetSingleCategory(9).ToList());
                            GoodsEquipment.AddRange(xy.GetSingleCategory(13).ToList());
                            GoodsEquipment.AddRange(xy.GetSingleCategory(14).ToList());
                            GoodsEquipment.AddRange(xy.GetSingleCategory(15).ToList());
                            GoodsEquipment.AddRange(xy.GetSingleCategory(16).ToList());
                            GoodsEquipment.AddRange(xy.GetSingleCategory(17).ToList());
                            GoodsEquipment.AddRange(xy.GetSingleCategory(18).ToList());
                            GoodsEquipment.AddRange(xy.GetSingleCategory(19).ToList());
                            GoodsEquipment.AddRange(xy.GetSingleCategory(20).ToList());
                            context.Add("GoodsEquipment", GoodsEquipment);
                        }

                        if (Form["btn_dj"].ToSafeString() != "")
                        {
                            List<ItemTemplateInfo> GoodsProps = xy.GetSingleCategory(11).ToList();
                            context.Add("GoodsProps", GoodsProps);
                        }

                        string hindGoodId = Form["hindGoodId"].ToSafeString();
                        if (Form["getGoodId"].ToSafeString() != "")
                        {
                            hindGoodId = Form["getGoodId"].ToSafeString();
                        }
                        string changParames = Form["changParames"].ToSafeString();
                        if (Form["params"].ToSafeString() != "")
                        {
                            changParames = Form["params"].ToSafeString();
                        }

                        if (hindGoodId != "" && changParames != "")
                        {
                            string[] paraStr = changParames.Split(new char[]
                            {
                    ','
                            });
                            if (tabGoods == null)
                            {
                                InitNewGoodTable();
                            }
                            int rowNum = Convert.ToInt32(hindGoodId);
                            DataRow[] rowArray = tabGoods.Select("id=" + hindGoodId);


                            DataRow[] array = rowArray;
                            for (int j = 0; j < array.Length; j++)
                            {
                                DataRow rows = array[j];
                                rows.BeginEdit();
                                rows["GoodId"] = hindGoodId.ToString();
                                rows["GoodNumber"] = paraStr[0].ToString();
                                rows["GoodName"] = paraStr[16].ToString();
                                rows["TemplateID"] = paraStr[12].ToString();
                                rows["ValidDate"] = paraStr[1].ToString();
                                rows["Gold"] = paraStr[10].ToString();
                                rows["Money"] = paraStr[9].ToString();
                                rows["LiJuan"] = paraStr[11].ToString();
                                rows["StrengthenLevel"] = paraStr[4].ToString();
                                rows["AttackCompose"] = paraStr[5].ToString();
                                rows["DefendCompose"] = paraStr[6].ToString();
                                rows["AgilityCompose"] = paraStr[7].ToString();
                                rows["LuckCompose"] = paraStr[8].ToString();
                                rows["IsBind"] = paraStr[2].ToString();
                                rows["Sex"] = paraStr[3].ToString();
                                rows["CategoryID"] = paraStr[13].ToString();
                                rows["CanStrengthen"] = paraStr[14].ToString();
                                rows["CanCompose"] = paraStr[15].ToString();
                                rows.EndEdit();
                            }

                        }
                        string delete = Form["deletegoodid"].ToSafeString();
                        if (delete != "")
                        {
                            if (tabGoods == null)
                            {
                                InitNewGoodTable();
                            }
                            tabGoods.Rows.Remove(tabGoods.Select("id=" + delete)[0]);
                        }




                        string idArray = Request.Uri.QueryString["ids"].ToSafeString();

                        if (idArray != "")
                        {

                            var a = idArray.Substring(0, idArray.Length - 1);
                            int[] b = a.Split(',').ConvertToIntArray();
                            List<ItemTemplateInfo> selectGoods = new List<ItemTemplateInfo>();
                            foreach (var c in b)
                            {
                                selectGoods.Add(xy.GetSingleGoods(c));
                            }

                            if (tabGoods == null)
                            {
                                InitNewGoodTable();
                            }

                            foreach (var item in selectGoods)
                            {
                                DataRow newRow = tabGoods.NewRow();
                                newRow["GoodId"] = item.TemplateID;
                                newRow["GoodNumber"] = 1;
                                newRow["GoodName"] = item.Name;
                                newRow["TemplateID"] = item.TemplateID;
                                newRow["ValidDate"] = 1;
                                newRow["StrengthenLevel"] = 0;
                                newRow["AttackCompose"] = 0;
                                newRow["DefendCompose"] = 0;
                                newRow["AgilityCompose"] = 0;
                                newRow["LuckCompose"] = 0;
                                newRow["IsBind"] = "True";
                                newRow["Sex"] = 0;
                                newRow["CategoryID"] = item.CategoryID;
                                newRow["CanStrengthen"] = item.CanStrengthen;
                                newRow["CanCompose"] = item.CanCompose;
                                tabGoods.Rows.Add(newRow);
                            }
                        }
                        if (tabGoods != null)
                        {
                            context.Add("ResultGoods", tabGoods);
                            if (!string.IsNullOrEmpty(idArray))
                            {
                                context.Add("isSelect", idArray.Substring(0, idArray.Length - 1));
                            }
                            else
                            {
                                context.Add("isSelect", "false");
                            }
                            idArray = "";
                        }

                        context.Add("this", new AdminPage());
                        Response.Write(iNVelocityEngine.Process(context, "Web.Server.Modules.Admin.Mail.vm"));
                    }
                }
            }
        }

        private static void InitNewGoodTable()
        {
            var goodsTable = new DataTable("goodsTable");
            DataColumn dc = goodsTable.Columns.Add("ID", Type.GetType("System.Int32"));
            dc.AutoIncrement = true;
            dc.AutoIncrementSeed = 1L;
            dc.AutoIncrementStep = 1L;
            dc.AllowDBNull = false;
            dc = goodsTable.Columns.Add("GoodId", Type.GetType("System.String"));
            dc = goodsTable.Columns.Add("GoodNumber", Type.GetType("System.String"));
            dc = goodsTable.Columns.Add("GoodName", Type.GetType("System.String"));
            dc = goodsTable.Columns.Add("TemplateID", Type.GetType("System.String"));
            dc = goodsTable.Columns.Add("ValidDate", Type.GetType("System.String"));
            dc = goodsTable.Columns.Add("Gold", Type.GetType("System.String"));
            dc = goodsTable.Columns.Add("Money", Type.GetType("System.String"));
            dc = goodsTable.Columns.Add("LiJuan", Type.GetType("System.String"));
            dc = goodsTable.Columns.Add("StrengthenLevel", Type.GetType("System.String"));
            dc = goodsTable.Columns.Add("AttackCompose", Type.GetType("System.String"));
            dc = goodsTable.Columns.Add("DefendCompose", Type.GetType("System.String"));
            dc = goodsTable.Columns.Add("AgilityCompose", Type.GetType("System.String"));
            dc = goodsTable.Columns.Add("LuckCompose", Type.GetType("System.String"));
            dc = goodsTable.Columns.Add("IsBind", Type.GetType("System.String"));
            dc = goodsTable.Columns.Add("Sex", Type.GetType("System.String"));
            dc = goodsTable.Columns.Add("CategoryID", Type.GetType("System.String"));
            dc = goodsTable.Columns.Add("CanStrengthen", Type.GetType("System.String"));
            dc = goodsTable.Columns.Add("CanCompose", Type.GetType("System.String"));
            tabGoods = goodsTable;
        }

        private static void ProcessNotice(IHttpRequest Request, IHttpResponse Response)
        {
            INVelocityEngine iNVelocityEngine = NVelocityEngineFactory.CreateNVelocityAssemblyEngine("Web.Server", false);
            IDictionary context = new Hashtable();
            Response.Write(iNVelocityEngine.Process(context, "Web.Server.Modules.Admin.SysNotice.vm"));
        }

        private static void ProcessIndex(IHttpRequest Request, IHttpResponse Response)
        {
            INVelocityEngine iNVelocityEngine = NVelocityEngineFactory.CreateNVelocityAssemblyEngine("Web.Server", false);
            IDictionary context = new Hashtable();
            Response.Write(iNVelocityEngine.Process(context, "Web.Server.Modules.Admin.Admin.vm"));
        }
        public static int CompareByID(UserInfo x, UserInfo y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                return 1;
            }
            else
            {
                if (y == null)
                {
                    return -1;
                }
                return -y.UserID.CompareTo(x.UserID);
            }
        }

        public string GetImage(int CategoryID, string pic)
        {
            return GetImage(CategoryID, pic, 0);
        }
        public string GetImage(int CategoryID, string pic, int sex)
        {
            var respath = "http://" + WebServer.Instance.Config.GameDomain + "/cdn/";
            switch (CategoryID)
            {
                case 1:
                    if (sex == 2)
                        return respath + @"image/equip/f/head/" + pic + "/icon_1.png";
                    else
                        return respath + @"image/equip/m/head/" + pic + "/icon_1.png";
                case 2:
                    if (sex == 2)
                        return respath + @"image/equip/f/glass/" + pic + "/icon_1.png";
                    else
                        return respath + @"image/equip/m/glass/" + pic + "/icon_1.png";
                case 3:
                    if (sex == 2)
                        return respath + @"image/equip/f/hair/" + pic + "/icon_1.png";
                    else
                        return respath + @"image/equip/m/hair/" + pic + "/icon_1.png";
                case 4:
                    if (sex == 2)
                        return respath + @"image/equip/f/eff/" + pic + "/icon_1.png";
                    else
                        return respath + @"image/equip/m/eff/" + pic + "/icon_1.png";
                case 5:
                    if (sex == 2)
                        return respath + @"image/equip/f/cloth/" + pic + "/icon_1.png";
                    else
                        return respath + @"image/equip/m/cloth/" + pic + "/icon_1.png";
                case 6:
                    if (sex == 2)
                        return respath + @"image/equip/f/face/" + pic + "/icon_1.png";
                    else
                        return respath + @"image/equip/m/face/" + pic + "/icon_1.png";
                case 7:
                    return respath + @"image/arm/" + pic + "/00.png";
                case 8:
                    return respath + @"image/equip/armlet/" + pic + "/icon.png";
                case 9:
                    return respath + @"image/equip/ring/" + pic + "/icon.png";
                case 11:
                    return respath + @"image/unfrightprop/" + pic + "/icon.png";
                case 13:
                    if (sex == 2)
                        return respath + @"image/equip/f/suits/" + pic + "/icon_1.png";
                    else
                        return respath + @"image/equip/m/suits/" + pic + "/icon_1.png";
                case 14:
                    return respath + @"image/equip/necklace/" + pic + "/icon.png";
                case 15:
                    return respath + @"image/equip/wing/" + pic + "/icon.png";
                case 16:
                    return respath + @"image/specialprop/chatBall/" + pic + "/icon.png";
                case 17:
                    return respath + @"image/equip/offhand/" + pic + "/icon.png";
                case 18:
                    return respath + @"image/equip/pet/" + pic + "/icon.png";
                case 19:
                case 20:
                    return respath + @"image/equip/shenqi/" + pic + "/icon.png";
                default:
                    return "";
            }
        }
    }
}
