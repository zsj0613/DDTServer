using System;
using Lsj.Util.Net.Web.Event;
using Lsj.Util.Net.Web.Interfaces;
using Lsj.Util.Net.Web.Error;
using Lsj.Util.Net.Web.Post;
using Lsj.Util;
using Lsj.Util.Text;
using System.Text;
using Lsj.Util.Net.Web.Message;
using System.Threading;
using Lsj.Util.Net.Web.Cookie;
using Bussiness;
using Lsj.Util.Net.Web;
using SqlDataProvider.Data;
using System.Linq;
using Web.Server.Manager;
using Lsj.Util.Logs;

namespace Web.Server.Modules
{
    internal class DDTAPIModule : IModule
    {
        string domain = WebServer.Instance.Config.Domain;
        


        public void Process(object website, ProcessEventArgs args)
        {
            var log = args.Log;
            
            var Request = args.Request;

            var Uri = Request.Uri;
            if (Uri.ToString().StartsWith("/api.do"))
            {
                args.IsProcessed = true;
                while (!Request.IsReadFinish)
                {
                    Thread.Sleep(100);
                }
                args.Response = new HttpResponse();
                var Response = args.Response;

                var referer = Request.Headers[eHttpHeader.Referer];
                var method = Uri.QueryString["method"];
                var postdata = Request.Content.ReadAll().ConvertFromBytes(Encoding.UTF8);


                var Form = FormParser.Parse(postdata);

                if (method == "login")
                {
                    #region login
                    if (referer == "")
                    {
                        Response.Write("非法访问");
                    }
                    var name = Form["user"];
                    var pass = Form["pass"];

                    using (var a = new MemberShipBussiness())
                    {
                        if (name == "" || pass == "" || !a.CheckUser(name, pass))
                        {
                            ((HttpResponse)Response).ReturnAndRedirect("用户名或密码错误", referer);
                        }
                        else
                        {
                            Response.Cookies.Add(new HttpCookie { name = "user", content = name, domain = domain, expires = DateTime.Now.AddHours(1) });
                            Response.Cookies.Add(new HttpCookie { name = "pass", content = pass, domain = domain, expires = DateTime.Now.AddHours(1) });
                            ((HttpResponse)Response).Redirect(referer.Replace("login.aspx", "") + "select.aspx");
                        }
                    }
                    #endregion
                }

                else if (method == "register")
                {
                    #region register
                    if (referer == "")
                    {
                        Response.Write("非法访问");
                    }
                    var name = Form["user"];
                    var pass = Form["pass"];
                    int inviteid = Request.Cookies["inviteid"].content.ConvertToInt(0);
                    using (var a = new MemberShipBussiness())
                    {
                        if (!a.ExistsUsername(name) && a.CreateUsername(name, pass, inviteid))
                        {
                            Response.Cookies.Add(new HttpCookie { name = "user", content = name, domain = domain, expires = DateTime.Now.AddHours(1) });
                            Response.Cookies.Add(new HttpCookie { name = "pass", content = pass, domain = domain, expires = DateTime.Now.AddHours(1) });
                            ((HttpResponse)Response).ReturnAndRedirect("注册成功", Request.Headers[eHttpHeader.Referer].Replace("login.aspx", "") + "select.aspx");
                        }
                        else
                        {
                            ((HttpResponse)Response).ReturnAndRedirect("该用户名已被注册", Request.Headers[eHttpHeader.Referer]);
                        }
                    }
                    #endregion
                }
                else if (method == "getuserid")
                {
                    #region getuserid
                    var name = Uri.QueryString["name"];
                    using (PlayerBussiness a = new PlayerBussiness())
                    {
                        PlayerInfo[] b = a.GetUserByUserName(name);
                        if (b != null)
                        {
                            var userid = b.Where((x) => (x.ID != 0)).Select((x) => (x.ID)).ToArray().FirstOrDefault();
                            if (userid != 0)
                            {
                                Response.Write(userid.ToString());
                                return;
                            }
                        }
                    }
                    Response.Write("0");
                    #endregion
                }
                else if (method == "isopen")
                {
                    Response.Write(WebServer.Instance.IsOpen.ToString());
                }
                else if (method == "checkuser")
                {
                    #region checkuser
                    var username = Form["username"];
                    var password = Form["password"];
                    var inviteid = Form["inviteid"].ConvertToInt(0);

                    using (var a = new MemberShipBussiness())
                    {
                        var result = a.CheckUser(username, password);
                        if (result)
                        {
                            using (var b = new PlayerBussiness())
                            {
                                b.CreateUsername(username, inviteid);
                                Response.Write("1");
                            }
                        }
                        else
                        {
                            Response.Write("0");
                        }
                    }
                    #endregion
                }
                else if (method == "addplayer")
                {
                    var user = Form["user"];
                    var pass = Form["pass"];
                    PlayerManager.Add(user, pass);
                    Response.Write("OK");
                }
                else if (method == "existsusername")
                {
                    var name = Form["name"];
                    using (var a = new MemberShipBussiness())
                    {
                        Response.Write(a.ExistsUsername(name).ToString());
                    }
                }
                else if (method == "getidbyusername")
                {
                    var username = Form["username"];
                    using (PlayerBussiness a = new PlayerBussiness())
                    {
                        PlayerInfo[] b = a.GetUserByUserName(username);
                        if (b != null)
                        {
                            Response.Write(b.Where((x) => (x.ID != 0)).Select((x) => (x.ID)).ToArray().FirstOrDefault().ToString());
                        }
                    }
                }
                else if (method == "getusertype")
                {
                    var username = Form["username"];
                    using (var a = new PlayerBussiness())
                    {
                        Response.Write(a.GetUserType(username).ToString());
                    }
                }
                else if (method == "getusernamebyid")
                {
                    var id = Form["id"].ConvertToInt(0);
                    using (PlayerBussiness a = new PlayerBussiness())
                    {
                        var b = a.GetUserSingleByUserID(id);
                        {
                            if (b != null)
                            {
                                Response.Write(b.UserName);
                            }
                        }
                    }
                }
                else
                {
                    args.Response = ErrorHelper.Build(404, 0, args.ServerName);
                }


            }
        }
    }
}