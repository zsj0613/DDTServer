using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using SqlDataProvider.Data;
using Lsj.Util;

public class WebHelperClient : ClientBase<IClient>, IClient
{
    public WebHelperClient() : base(new WSHttpBinding("httpbinding"), new EndpointAddress(new Uri(WebHelper.AppSettings["WCFService"])))
    {
        
    }

    public void AddPlayer(string user, string pass)
    {
        base.Channel.AddPlayer(user, pass);
    }

    public bool CheckUser(string username, string password,int inviteid)
    {
        return base.Channel.CheckUser(username, password, inviteid);
    }

    public bool ExistsUsername(string name)
    {
        return base.Channel.ExistsUsername(name);
    }

    public UserInfo[] GetAllUserInfo()
    {
        return base.Channel.GetAllUserInfo();
    }

    public int[] GetIDByUserName(string username)
    {
        return base.Channel.GetIDByUserName(username);
    }

    public bool[] GetRunMgr()
    {
        return base.Channel.GetRunMgr();
    }

    public ItemTemplateInfo[] GetSingleCategoryItemTemplates(int i)
    {
        return base.Channel.GetSingleCategoryItemTemplates(i);
    }

    public ItemTemplateInfo GetSingleItemTemplate(int i)
    {
        return base.Channel.GetSingleItemTemplate(i);
    }

    public string GetUserNameByID(int userid)
    {
        return base.Channel.GetUserNameByID(userid);
    }

    public int GetUserType(string name)
    {
        return base.Channel.GetUserType(name);
    }

    public bool IsOpen()
    {
        return base.Channel.IsOpen();
    }

    public string Login(string p, string site, string IP)
    {
        return base.Channel.Login(p, site, IP);
    }

    public bool Register(string username, string password, int inviteid)
    {
        return base.Channel.Register(username, password, inviteid);
    }

    public void SendMailAndItem(string title, string content, int b, int gold, int money, int giftToken, string str)
    {
        base.Channel.SendMailAndItem(title, content, b, gold, money, giftToken, str);
    }

    public bool Reconnect()
    {
        return base.Channel.Reconnect();
    }

    public string GMAction(string action, Dictionary<string, string> param)
    {
        return base.Channel.GMAction(action, param);
    }

    public string ServerList()
    {
        return base.Channel.ServerList();
    }

    public void MailNotice(int userid)
    {
        base.Channel.MailNotice(userid);
    }
}