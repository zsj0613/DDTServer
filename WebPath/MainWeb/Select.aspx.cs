using Lsj.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Select : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.Cookies["user"].GetSafeValue() != "" && Request.Cookies["pass"].GetSafeValue() != "")
        {
            Response.WriteFile("vm/select.vm");
        }
        else
        {
            Response.Redirect("login.aspx");
        }
    }
}