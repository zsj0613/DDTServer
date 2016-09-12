<%@ WebHandler Language="C#" Class="ServerList" %>

using Bussiness;
using Bussiness.CenterService;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;

[WebService(Namespace = "http://tempuri.org/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class ServerList : IHttpHandler
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
        using (var a = new WebHelperClient())
        {

            context.Response.ContentType = "text/plain";
            context.Response.Write(a.ServerList());
        }
    }
}

