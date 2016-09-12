<%@ WebHandler Language="C#" Class="LoadUserEquip" Debug="true"%>

using Bussiness;

using SqlDataProvider.Data;
using System;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;

[WebService(Namespace = "http://tempuri.org/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class LoadUserEquip : IHttpHandler
{

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
    public void ProcessRequest(HttpContext context)
    {
        bool value = false;
        string message = "Fail!";
        XElement result = new XElement("Result");
        //try
        //{
        int userid = int.Parse(context.Request["ID"]);
        using (PlayerBussiness pb = new PlayerBussiness())
        {
            using(var a = new WebHelperClient())
            {
                PlayerInfo info = pb.GetUserSingleByUserID(userid);
                result.Add(new object[]
                {
                        new XAttribute("Agility", info.Agility),
                        new XAttribute("Attack", info.Attack),
                        new XAttribute("Colors", info.Colors),
                        new XAttribute("Skin", info.Skin),
                        new XAttribute("Defence", info.Defence),
                        new XAttribute("GP", info.GP),
                        new XAttribute("Grade", info.Grade),
                        new XAttribute("Luck", info.Luck),
                        new XAttribute("Hide", info.Hide),
                        new XAttribute("Repute", info.Repute),
                        new XAttribute("Offer", info.Offer),
                        new XAttribute("NickName", info.NickName),
                        new XAttribute("ConsortiaName", info.ConsortiaName),
                        new XAttribute("ConsortiaID", info.ConsortiaID),
                        new XAttribute("ReputeOffer", info.ReputeOffer),
                        new XAttribute("ConsortiaHonor", info.ConsortiaHonor),
                        new XAttribute("ConsortiaLevel", info.ConsortiaLevel),
                        new XAttribute("ConsortiaRepute", info.ConsortiaRepute),
                        new XAttribute("WinCount", info.Win),
                        new XAttribute("TotalCount", info.Total),
                        new XAttribute("EscapeCount", info.Escape),
                        new XAttribute("Sex", info.Sex),
                        new XAttribute("Style", info.Style),
                        new XAttribute("FightPower", info.FightPower),
                        new XAttribute("LastSpaDate", info.LastSpaDate),
                        new XAttribute("IsGM", a.GetUserType(info.UserName)>=2?"True":"False"),
                        new XAttribute("VIPLevel",info.VipLevel),
                });
                ItemInfo[] items = pb.GetUserEuqip(userid).ToArray();
                ItemInfo[] array = items;
                for (int i = 0; i < array.Length; i++)
                {
                    ItemInfo g = array[i];
                    result.Add(FlashUtils.CreateGoodsInfo(g));
                }
            }
        }
        value = true;
        message = "Success!";
        //}
        //catch (Exception ex)
        //{
        //	LoadUserEquip.log.Error("LoadUserEquip", ex);
        //}
        result.Add(new XAttribute("value", value));
        result.Add(new XAttribute("message", message));
        context.Response.ContentType = "text/plain";
        context.Response.Write(result.ToString(false));
    }
}