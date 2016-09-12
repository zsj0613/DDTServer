using System;
using System.Collections.Generic;
using System.Web;
using Lsj.Util;
using Lsj.Util.Collections;
using Lsj.Util.Net.Web;
using Lsj.Util.Net.Web.Post;
using Lsj.Util.Text;

public class WebHelperClient :DisposableClass,IDisposable
{
    WebHttpClient client;
    public WebHelperClient() 
    {
        this.client = new WebHttpClient();
    }

    protected override void CleanUpManagedResources()
    {
        //client.Dispose();
        client = null;
    }



    public void AddPlayer(string user, string pass)
    {
        try
        {
            client.Post($"{WebHelper.AppSettings["InternalWeb"]}?method=addplayer",
                FormParser.ToBytes(new SafeStringToStringDirectionary
                {
                    {"user",user },
                    {"pass",pass },
                }
                ));
        }
        catch
        {
        }
    }

    public bool CheckUser(string username, string password, int inviteid)
    {
        try
        {
            var result = client.Post($"{WebHelper.AppSettings["InternalWeb"]}?method=checkuser",
                FormParser.ToBytes(new SafeStringToStringDirectionary
                {
                    {"username",username },
                    {"password",password },
                    {"inviteid",inviteid.ToString() },
                }
                )).ConvertFromBytes().ConvertToInt(0);
            return result == 1;
        }
        catch
        {
            return false;
        }
    }

    public bool ExistsUsername(string name)
    {
        try
        {
            var result = client.Post($"{WebHelper.AppSettings["InternalWeb"]}?method=existsusername",
                FormParser.ToBytes(new SafeStringToStringDirectionary
                {
                    {"name",name },
                }
                )).ConvertFromBytes();
            return result == "True";
        }
        catch
        {
            return true;
        }
    }

    public int GetIDByUserName(string username)
    {
        try
        {
            var result = client.Post($"{WebHelper.AppSettings["InternalWeb"]}?method=getidbyusername",
                FormParser.ToBytes(new SafeStringToStringDirectionary
                {
                    {"username",username },
                }
                )).ConvertFromBytes().ConvertToInt();
            return result;
        }
        catch
        {
            return 0;
        }
    }

    public string GetUserNameByID(int id)
    {
        try
        {
            var result = client.Post($"{WebHelper.AppSettings["InternalWeb"]}?method=getusernamebyid",
                FormParser.ToBytes(new SafeStringToStringDirectionary
                {
                    {"id",id.ToString() },
                }
                )).ConvertFromBytes();
            return result;
        }
        catch
        {
            return "";
        }
    }

    public int GetUserType(string username)
    {
        try
        {
            var result = client.Post($"{WebHelper.AppSettings["InternalWeb"]}?method=getusertype",
                FormParser.ToBytes(new SafeStringToStringDirectionary
                {
                    {"username",username },
                }
                )).ConvertFromBytes().ConvertToInt();
            return result;
        }
        catch
        {
            return 0;
        }
    }

    public bool IsOpen()
    {
        try
        {
            var result = client.Get($"{WebHelper.AppSettings["InternalWeb"]}?method=isopen").ConvertFromBytes();
            return result == "True";
        }
        catch
        {
            return false;
        }
    }
}