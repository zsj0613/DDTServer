using Lsj.Util.Net.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Web.Server
{
    public interface IModule
    {
        HttpClient Process(HttpClient request);
    }
}
