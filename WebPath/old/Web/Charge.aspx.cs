using Lsj.Util;
using Lsj.Util.Config;
using NVelocityTemplateEngine;
using NVelocityTemplateEngine.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Charge : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["step"].ConvertToInt(0) == 1)
        {
            ProcessStep1();
        }
        else if (Request.QueryString["step"].ConvertToInt(0) == 2)
        {
            ProcessStep2();
        }
        else
        {
            Write404();
        }
    }
    void Write404()
    {
        Response.StatusCode = 404;
        Response.Write("404 Not Found");
    }


    private void ProcessStep1()
    {
        string userid = Request.QueryString["userid"];
        if (userid.ConvertToInt(0) == 0)
        {
            Write404();
        }
        else
        {
            INVelocityEngine FileEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(AppConfig.AppSettings["Path"], false);
            IDictionary context = new Hashtable();
            context.Add("userid", userid);
            Response.Write(FileEngine.Process(context, @"charge\PayIndex.vm"));
        }
    }

    private void ProcessStep2()
    {
        string userid = Request.QueryString["userid"];
        string paytype = Request.QueryString["paytype"];
        if (paytype.ConvertToInt(0) == 0 || userid.ConvertToInt(0) == 0)
        {
            Write404();
        }

        if (paytype.ConvertToInt(0) >= 1 && paytype.ConvertToInt(0) <= 24)
        {
            string username = "";
            using (var a = new WebHelperClient())
            {
                var x = a.GetUserNameByID(userid.ConvertToInt(0));
                if (x != "")
                {
                    username = x;
                }
                else
                {
                    Write404();
                }
            }


            INVelocityEngine FileEngine = NVelocityEngineFactory.CreateNVelocityFileEngine(AppConfig.AppSettings["Path"], false);
            IDictionary context = new Hashtable();

            context.Add("userid", userid);
            context.Add("username", username);
            Response.Write(FileEngine.Process(context, "charge/Pay"+paytype+".vm"));
        }
        else
        {
            Write404();
        }
    }
}