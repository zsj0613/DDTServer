using Bussiness;
using Game.Base.Packets;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Center.Server
{
    public class RenameMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
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
                
            }
        }

    }
}
