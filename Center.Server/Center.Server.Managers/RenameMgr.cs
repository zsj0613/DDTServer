using Bussiness;
using Game.Base.Packets;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Center.Server.Managers
{
    public class RenameMgr
    {
        private static LogProvider log => CenterServer.log;
        public static void Do()
        {
            try {
                using (var db = new ManageBussiness())
                {
                    db.UpdateName();
                }
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }
        }

    }
}
