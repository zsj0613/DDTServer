using log4net;
using Lsj.Util.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Reflection;

namespace Game.Core
{
    public class BaseServer : TcpSyncServer
    {
        private static readonly int SEND_BUFFER_SIZE = 16384;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static BaseServer Create(EndPoint endpoint)
        {
            return new BaseServer(endpoint);
        }
        private BaseServer(EndPoint endpoint):base(endpoint)
        {
        }
        public override void Start()
        {
            try
            {
                base.Start();
                log.Info("Server Started");
            }
            catch(Exception e)
            {
                if (this.m_socket != null)
                    this.m_socket.Close();
                log.Error("Start Server Error", e);
            }
        }

        protected override TcpSocket OnAccept(TcpSocket handle)
        {
            handle.SendBufferSize = SEND_BUFFER_SIZE;
            return handle;
        }


    }
}