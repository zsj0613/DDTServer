using Lsj.Util;
using Lsj.Util.Config;
using NVelocityTemplateEngine;
using NVelocityTemplateEngine.Interfaces;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class admin_admin : System.Web.UI.Page
{
    private string username;
    private string password;
    private int usertype = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        this.username = Request.Cookies["username"].GetSafeValue();
        this.password = Request.Cookies["password"].GetSafeValue();
        int inviteid = Request.Cookies["inviteid"].GetSafeValue().ConvertToInt(0);

        using (var a = new WebHelperClient())
        {
            if (a.CheckUser(username, password,inviteid))
            {
                this.usertype = a.GetUserType(username);
            }
        }

        if (this.usertype > 1)
        {
            var page = this.Request.QueryString["page"].ToSafeString();
            switch (page)
            {
                case "left":
                    ProcessLeft();
                    break;
                case "userlist":
                    ProcessUserlist();
                    break;
                case "version":
                    ProcessVersion();
                    break;
                case "top":
                    ProcessTop();
                    break;
                case "mail":
                    ProcessMail();
                    break;
                case "notice":
                    ProcessNotice();
                    break;
                case "status":
                    ProcessStatus();
                    break;
                default:
                    ProcessIndex();
                    break;
            }
        }
        else
        {
            Response.ReturnAndRedirect("你无权访问GM管理平台！", "../login");
        }
    }

    private void ProcessIndex()
    {
        INVelocityEngine iNVelocityEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(AppConfig.AppSettings["Path"], false);
        IDictionary context = new Hashtable();
        Response.Write(iNVelocityEngine.Process(context, "Admin.vm"));
    }

    private void ProcessNotice()
    {
        INVelocityEngine iNVelocityEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(AppConfig.AppSettings["Path"], false);
        IDictionary context = new Hashtable();
        Response.Write(iNVelocityEngine.Process(context, "SysNotice.vm"));
    }

    DataTable goodsTable;
    private void ProcessMail()
    {
        if (!(this.usertype > 2))
        {
            Response.Write("对不起,你的权限不足");
            return;
        }
        using (var xx = new WebHelperClient())
        {

            if (Request["sub_searchUserId"].ToSafeString() != "")
            {
                string userMsg = Request["userMsg"].ToSafeString();
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
            else if (Request.Form["txt_userID"].ToSafeString() != "" && Request.Form["txt_Title"].ToSafeString() != "" && Request.Form["txt_Content"].ToSafeString() != "")
            {
                if (SendMail())
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
                INVelocityEngine FileEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(AppConfig.AppSettings["Path"], false);
                IDictionary context = new Hashtable();
                if (Request["btn_wq"].ToSafeString() != "")
                {
                    List<ItemTemplateInfo> GoodsWeapons = xx.GetSingleCategoryItemTemplates(7).ToList();
                    context.Add("GoodsWeapons", GoodsWeapons);
                }
                if (Request["btn_zb"].ToSafeString() != "")
                {
                    List<ItemTemplateInfo> GoodsEquipment = xx.GetSingleCategoryItemTemplates(1).ToList();
                    GoodsEquipment.AddRange(xx.GetSingleCategoryItemTemplates(2).ToList());
                    GoodsEquipment.AddRange(xx.GetSingleCategoryItemTemplates(3).ToList());
                    GoodsEquipment.AddRange(xx.GetSingleCategoryItemTemplates(4).ToList());
                    GoodsEquipment.AddRange(xx.GetSingleCategoryItemTemplates(5).ToList());
                    GoodsEquipment.AddRange(xx.GetSingleCategoryItemTemplates(6).ToList());
                    GoodsEquipment.AddRange(xx.GetSingleCategoryItemTemplates(8).ToList());
                    GoodsEquipment.AddRange(xx.GetSingleCategoryItemTemplates(9).ToList());
                    GoodsEquipment.AddRange(xx.GetSingleCategoryItemTemplates(13).ToList());
                    GoodsEquipment.AddRange(xx.GetSingleCategoryItemTemplates(14).ToList());
                    GoodsEquipment.AddRange(xx.GetSingleCategoryItemTemplates(15).ToList());
                    GoodsEquipment.AddRange(xx.GetSingleCategoryItemTemplates(16).ToList());
                    GoodsEquipment.AddRange(xx.GetSingleCategoryItemTemplates(17).ToList());
                    GoodsEquipment.AddRange(xx.GetSingleCategoryItemTemplates(18).ToList());
                    GoodsEquipment.AddRange(xx.GetSingleCategoryItemTemplates(19).ToList());
                    GoodsEquipment.AddRange(xx.GetSingleCategoryItemTemplates(20).ToList());
                    context.Add("GoodsEquipment", GoodsEquipment);
                }

                if (Request["btn_dj"].ToSafeString() != "")
                {
                    List<ItemTemplateInfo> GoodsProps = xx.GetSingleCategoryItemTemplates(11).ToList();
                    context.Add("GoodsProps", GoodsProps);
                }

                string hindGoodId = Request["hindGoodId"].ToSafeString();
                if (Request["getGoodId"].ToSafeString() != "")
                {
                    hindGoodId = Request["getGoodId"].ToSafeString();
                    ;
                }
                string changParames = Request["changParames"].ToSafeString();
                if (Request["params"].ToSafeString() != "")
                {
                    changParames = Request["params"].ToSafeString();
                }

                if (hindGoodId != "" && changParames != "")
                {
                    string[] paraStr = changParames.Split(new char[]
                    {
                    ','
                    });
                    DataTable tabGoods = Context.Session["goodsTable"] as DataTable ?? InitNewGoodTable();
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
                    Context.Session["goodsTable"] = tabGoods;

                }
                string delete = Request["deletegoodid"].ToSafeString();
                if (delete != "")
                {
                    DataTable dt = Context.Session["goodsTable"] as DataTable ?? InitNewGoodTable();
                    dt.Rows.Remove(dt.Select("id=" + delete)[0]);
                }




                string idArray = Request["ids"].ToSafeString();

                if (idArray != "")
                {
                    
                    var a = idArray.Substring(0, idArray.Length - 1);
                    int[] b = a.Split(',').ConvertToIntArray();
                    List<ItemTemplateInfo> selectGoods = new List<ItemTemplateInfo>();
                    foreach (var c in b)
                    {
                        selectGoods.Add(xx.GetSingleItemTemplate(c));
                    }

                    if (Context.Session["goodsTable"] == null)
                    {
                        goodsTable=InitNewGoodTable();
                    }
                    else
                    {
                        goodsTable = Context.Session["goodsTable"] as DataTable;
                    }
                    foreach (var item in selectGoods)
                    {
                        DataRow newRow = goodsTable.NewRow();
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
                        goodsTable.Rows.Add(newRow);
                    }
                    Context.Session["goodsTable"] = goodsTable;
                }
                if (Context.Session["goodsTable"] != null)
                {
                    context.Add("ResultGoods", Context.Session["goodsTable"] as DataTable);
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

                context.Add("this", this);
                Response.Write(FileEngine.Process(context, "Mail.vm"));
            }
        }
    }

    private DataTable InitNewGoodTable()
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
        return goodsTable;
    }

    private void ProcessTop()
    {
        INVelocityEngine iNVelocityEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(AppConfig.AppSettings["Path"], false);
        IDictionary dictionary = new Hashtable();
        dictionary.Add("UserName", this.username);
        Response.Write(iNVelocityEngine.Process(dictionary, "Admin_Top.vm"));
    }

    private void ProcessVersion()
    {
        INVelocityEngine iNVelocityEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(AppConfig.AppSettings["Path"], false);
        IDictionary context = new Hashtable();
        Response.Write(iNVelocityEngine.Process(context, "Admin_Version.vm"));
    }

    private void ProcessUserlist()
    {
        string search = Request.Form["Tb_SearchKeys"].ToSafeString();
        int num = StringHelper.ConvertToInt(Request.QueryString["pages"], 1);
        INVelocityEngine arg_13D_0 = NVelocityEngineFactory.CreateNVelocityFileEngine(AppConfig.AppSettings["Path"], false);
        IDictionary dictionary = new Hashtable();
        using (var x = new WebHelperClient())
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
            dictionary.Add("Result", list);
            dictionary.Add("OnlineCount",onlinecount);
            dictionary.Add("Search", search);
            dictionary.Add("Count", count);
            dictionary.Add("Page", num);
            dictionary.Add("TocalPage", num2);
            string text = arg_13D_0.Process(dictionary, "UserList.vm");
            Response.Write(text);
        }
           
    }
    private void ProcessStatus()
    {
        using (var a = new WebHelperClient())
        {

            INVelocityEngine iNVelocityEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(AppConfig.AppSettings["Path"], false);
            IDictionary dictionary = new Hashtable();
            dictionary.Add("Runmgr", a.GetRunMgr());
            dictionary.Add("IsConnected", a.IsOpen());
            Response.Write(iNVelocityEngine.Process(dictionary, "Status.vm"));
        }
    }
    private void ProcessLeft()
    {
        INVelocityEngine iNVelocityEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(AppConfig.AppSettings["Path"], false);
        IDictionary context = new Hashtable();
        Response.Write(iNVelocityEngine.Process(context, "Admin_Left.vm"));
    }

    private void Write404()
    {
        Response.StatusCode = 404;
        Response.Write("404 Not Found");
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
        var respath = "../cdn/";
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
    private bool SendMail()
    {
        if (!(this.usertype > 2))
        {
     
            return false;
        }
        DataTable tabGoods = Context.Session["goodsTable"] as DataTable;
        bool result = false;
        var Title = System.Web.HttpUtility.UrlDecode(Request.Form["txt_Title"].ToSafeString());
        var Gold = Request.Form["txt_gold"].ConvertToInt();
        var Money = Request.Form["txt_money"].ConvertToInt();
        var GiftToken = Request.Form["txt_liJuan"].ConvertToInt();
        var Content = System.Web.HttpUtility.UrlDecode(Request.Form["txt_Content"].ToSafeString());
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
        int[] IDs = Request.Form["txt_userID"].Split(',').ConvertToIntArray();
        using (var a = new WebHelperClient())
        {
            foreach (var b in IDs)
            {
                a.SendMailAndItem(Title, Content, b, Gold, Money, GiftToken, str);
            }
        }
        result = true;
        Context.Session["goodsTable"] = null;
        return result;

    }
}