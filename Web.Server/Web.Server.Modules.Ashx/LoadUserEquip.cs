
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Bussiness;
using Lsj.Util.Net.Web;
using Lsj.Util.Net.Web.Interfaces;
using SqlDataProvider.Data;

namespace Web.Server.Modules.Ashx
{
    public class LoadUserEquip
    {
        public static void Process(IHttpRequest Request, IHttpResponse Response)
        {

            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            int userid = int.Parse(Request.Uri.QueryString["ID"]);
            using (PlayerBussiness pb = new PlayerBussiness())
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
                        new XAttribute("IsGM", pb.GetUserType(info.UserName)>=2?"True":"False"),
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
            value = true;
            message = "Success!";
            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            Response.Write(result.ToString(false));
        }
       
    }
}
