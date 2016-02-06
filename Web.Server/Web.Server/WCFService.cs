using Lsj.Util.Config;
using Lsj.Util.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Web.Server
{
    public class WCFService : IWCFService
    {
        static LogProvider log => LogProvider.Default;
        private static ServiceHost host;
        public static bool Start()
        {
            bool result;
            try
            {
                WCFService.host = new ServiceHost(typeof(WCFService), new Uri[] { new Uri(AppConfig.AppSettings["WCFBinding"])});
                WCFService.host.Open();
                WCFService.log.Info("Center Service started!");
                result = true;
            }
            catch (Exception ex)
            {
                WCFService.log.ErrorFormat("Start center server failed:{0}", ex);
                result = false;
            }
            return result;
        }
        public static void Stop()
        {
            try
            {
                if (WCFService.host != null)
                {
                    WCFService.host.Close();
                    WCFService.host = null;
                }
            }
            catch
            {
            }
        }
    }
}
