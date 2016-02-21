<%@ WebHandler Language="C#" Class="Login" %>

using System;
using System.Web;

public class Login : IHttpHandler {

    public void ProcessRequest (HttpContext context)
    {
        string p = context.Request.QueryString["p"];
        string site = (context.Request.QueryString["site"] == null) ? "" : context.Request.QueryString["site"];
        string IP = context.Request.UserHostAddress;
        using (var a = new WebHelperClient())
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(a.Login(p,site, IP));
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}