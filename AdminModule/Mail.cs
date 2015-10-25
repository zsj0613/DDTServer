using Bussiness;
using Lsj.Util;
using Lsj.Util.Config;
using Lsj.Util.Net.Web;
using Lsj.Util.Net.Web.Response;
using NVelocityTemplateEngine;
using NVelocityTemplateEngine.Interfaces;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Web.Server.Module
{
    public class Mail
    {
        DataTable goodsTable;
        public void ProcessMail(ref HttpResponse response, HttpRequest request)
        {
            if (request["sub_searchUserId"] != "")
            {
                string userMsg = request["userMsg"];
                string getAllUserId = "";
                if (userMsg != "")
                {
                    string[] strUsersg = userMsg.Split(new char[]
                    {
                        ','
                    });
                    List<UserInfo> result = new ManageBussiness().GetAllUserInfo();
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
                    response.Write(getAllUserId);
                    return;
                }
            }
            else if (request.Form["txt_userID"]!="" && request.Form["txt_Title"]!="" && request.Form["txt_Content"] != "")
            {
                if (SendMail(request))
                {
                    response.Write("成功");
                }
                else
                {
                    response.Write("失败");
                }
            }
            else
            {
                INVelocityEngine FileEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(Server.ModulePath + @"vm", false);
                IDictionary context = new Hashtable();
                if (request["btn_wq"] != "")
                {
                    List<ItemTemplateInfo> GoodsWeapons = new ProduceBussiness().GetSingleCategory(7).ToList();
                    context.Add("GoodsWeapons", GoodsWeapons);
                }
                if (request["btn_zb"] != "")
                {
                    var a = new ProduceBussiness();
                    List<ItemTemplateInfo> GoodsEquipment = a.GetSingleCategory(1).ToList();
                    GoodsEquipment.AddRange(a.GetSingleCategory(2).ToList());
                    GoodsEquipment.AddRange(a.GetSingleCategory(3).ToList());
                    GoodsEquipment.AddRange(a.GetSingleCategory(4).ToList());
                    GoodsEquipment.AddRange(a.GetSingleCategory(5).ToList());
                    GoodsEquipment.AddRange(a.GetSingleCategory(6).ToList());
                    GoodsEquipment.AddRange(a.GetSingleCategory(8).ToList());
                    GoodsEquipment.AddRange(a.GetSingleCategory(9).ToList());
                    GoodsEquipment.AddRange(a.GetSingleCategory(13).ToList());
                    GoodsEquipment.AddRange(a.GetSingleCategory(14).ToList());
                    GoodsEquipment.AddRange(a.GetSingleCategory(15).ToList());
                    GoodsEquipment.AddRange(a.GetSingleCategory(16).ToList());
                    GoodsEquipment.AddRange(a.GetSingleCategory(17).ToList());
                    GoodsEquipment.AddRange(a.GetSingleCategory(18).ToList());
                    GoodsEquipment.AddRange(a.GetSingleCategory(19).ToList());
                    GoodsEquipment.AddRange(a.GetSingleCategory(20).ToList());
                    context.Add("GoodsEquipment", GoodsEquipment);
                }

                if (request["btn_dj"] != "")
                {
                    List<ItemTemplateInfo> GoodsProps = new ProduceBussiness().GetSingleCategory(11).ToList();
                    context.Add("GoodsProps", GoodsProps);
                }

                string hindGoodId = request["hindGoodId"];
                if (request["getGoodId"]!="")
                {
                    hindGoodId = request["getGoodId"]; ;
                }
                string changParames = request["changParames"];                
                if (request["params"]!="")
                {
                    changParames = request["params"];
                }

                if (hindGoodId!=""&& changParames!="")
                {
                    string[] paraStr = changParames.Split(new char[]
                    {
                    ','
                    });
                    DataTable tabGoods = request.Session["goodsTable"] as DataTable;
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
                    request.Session["goodsTable"] = tabGoods;

                }
                string delete = request["deletegoodid"];
                if (delete!="")
                {
                    DataTable dt = request.Session["goodsTable"] as DataTable;
                    dt.Rows.Remove(dt.Select("id=" + delete)[0]);
                }




                string idArray = request["ids"];

                if (idArray!="")
                {
                    var d = new ProduceBussiness();
                    var a = idArray.Substring(0, idArray.Length - 1);
                    int[] b = a.Split(',').ConvertToIntArray();
                    List<ItemTemplateInfo> selectGoods = new List<ItemTemplateInfo>();
                    foreach (var c in b)
                    {
                        selectGoods.Add(d.GetSingleGoods(c));
                    }
                    
                    if (request.Session["goodsTable"] == null)
                    {
                        goodsTable = new DataTable("goodsTable");
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
                        dc =goodsTable.Columns.Add("IsBind", Type.GetType("System.String"));
                        dc =goodsTable.Columns.Add("Sex", Type.GetType("System.String"));
                        dc =goodsTable.Columns.Add("CategoryID", Type.GetType("System.String"));
                        dc =goodsTable.Columns.Add("CanStrengthen", Type.GetType("System.String"));
                        dc =goodsTable.Columns.Add("CanCompose", Type.GetType("System.String"));
                    }
                    else
                    {
                       goodsTable = request.Session["goodsTable"] as DataTable;
                    }
                    foreach (var item in selectGoods)
                    {
                        DataRow newRow =goodsTable.NewRow();
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
                   request.Session["goodsTable"] =goodsTable;
                }
                if (request.Session["goodsTable"] != null)
                {
                    context.Add("ResultGoods", request.Session["goodsTable"] as DataTable);
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
                response.Write(FileEngine.Process(context, "Mail.vm"));
            }
        }
        private bool SendMail(HttpRequest request)
        {
            DataTable tabGoods = request.Session["goodsTable"] as DataTable;
            bool result = false;
            var Title = System.Web.HttpUtility.UrlDecode(request.Form["txt_Title"]);
            var Gold = request.Form["txt_gold"].ConvertToInt();
            var Money = request.Form["txt_money"].ConvertToInt();
            var GiftToken = request.Form["txt_liJuan"].ConvertToInt();
            var Content = System.Web.HttpUtility.UrlDecode(request.Form["txt_Content"]);
            var str = "";
            if (tabGoods != null)
            {
                foreach(DataRow x in tabGoods.Rows)
                {
                   str += $"{x["TemplateId"]},{x["GoodNumber"]},{x["ValidDate"]},{x["StrengthenLevel"]},{x["AttackCompose"]},{x["DefendCompose"]},{x["AgilityCompose"]},{x["LuckCompose"]},{((x["IsBind"].ToString() == "True")?1:0)}|";
                }
                str.RemoveLastOne();
            }
            WebServer.log.Debug(str);
            int[] IDs = request.Form["txt_userID"].Split(',').ConvertToIntArray();
            PlayerBussiness a = new PlayerBussiness();
            foreach (var b in IDs)
            {
                a.SendMailAndItem(Title, Content, b, Gold, Money, GiftToken, str);
            }
            result = true;
            request.Session["goodsTable"] = null;
            return result;

        }
        public string GetImage(int CategoryID, string pic)
        {
            return GetImage(CategoryID, pic, 0);
        }
        public string GetImage(int CategoryID, string pic, int sex)
        {
            var respath = AppConfig.AppSettings["ResPath"];
            switch (CategoryID)
            {
                case 1:
                    if (sex == 2)
                        return respath+@"image/equip/f/head/" + pic + "/icon_1.png";
                    else
                        return respath+@"image/equip/m/head/" + pic + "/icon_1.png";
                case 2:
                    if (sex == 2)
                        return respath+@"image/equip/f/glass/" + pic + "/icon_1.png";
                    else
                        return respath+@"image/equip/m/glass/" + pic + "/icon_1.png";
                case 3:
                    if (sex == 2)
                        return respath+@"image/equip/f/hair/" + pic + "/icon_1.png";
                    else
                        return respath+@"image/equip/m/hair/" + pic + "/icon_1.png";
                case 4:
                    if (sex == 2)
                        return respath+@"image/equip/f/eff/" + pic + "/icon_1.png";
                    else
                        return respath+@"image/equip/m/eff/" + pic + "/icon_1.png";
                case 5:
                    if (sex == 2)
                        return respath+@"image/equip/f/cloth/" + pic + "/icon_1.png";
                    else
                        return respath+@"image/equip/m/cloth/" + pic + "/icon_1.png";
                case 6:
                    if (sex == 2)
                        return respath+@"image/equip/f/face/" + pic + "/icon_1.png";
                    else
                        return respath+@"image/equip/m/face/" + pic + "/icon_1.png";
                case 7:
                    return respath+@"image/arm/" + pic + "/00.png";
                case 8:
                    return respath+@"image/equip/armlet/" + pic + "/icon.png";
                case 9:
                    return respath+@"image/equip/ring/" + pic + "/icon.png";
                case 11:
                    return respath+@"image/unfrightprop/" + pic + "/icon.png";
                case 13:
                    if (sex == 2)
                        return respath+@"image/equip/f/suits/" + pic + "/icon_1.png";
                    else
                        return respath+@"image/equip/m/suits/" + pic + "/icon_1.png";
                case 14:
                    return respath+@"image/equip/necklace/" + pic + "/icon.png";
                case 15:
                    return respath+@"image/equip/wing/" + pic + "/icon.png";
                case 16:
                    return respath+@"image/specialprop/chatBall/" + pic + "/icon.png";
                case 17:
                    return respath+@"image/equip/offhand/" + pic + "/icon.png";
                case 18:
                    return respath+@"image/equip/pet/" + pic + "/icon.png";
                case 19:
                case 20:
                    return respath+@"image/equip/shenqi/" + pic + "/icon.png";
                default:
                    return "";
            }
        }
    }
}
