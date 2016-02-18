using Bussiness;
using Bussiness.Managers;
using Game.Base;
using Game.Base.Packets;
using Lsj.Util.Config;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Center.Server
{
	public class CrossServerConnector : BaseConnector
	{
		private new static LogProvider log => LogProvider.Default;

		private string key;
		protected override void OnConnect()
		{
			base.OnConnect();
			this.m_privLevel = ePrivLevel.Admin;
		}
		protected override void OnDisconnect()
		{
			base.OnDisconnect();
		}
		public override void OnRecvPacket(GSPacketIn pkg)
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.AsynProcessPacket), pkg);
		}
		protected void AsynProcessPacket(object state)
		{
			try
			{
				GSPacketIn pkg = state as GSPacketIn;
				short code = pkg.Code;
                switch (code)
                {
                    case 0:
                        this.HandleRSAKey(pkg);
                        break;
                    case 25:
                        this.HandleAreaBigBugle(pkg);
                        break;
                    default:
                        break;
                }
			}
			catch (Exception ex)
			{
				CenterServer.log.Error("AsynProcessPacket", ex);
			}
		}

        private void HandleAreaBigBugle(GSPacketIn pkg)
        {
            CenterServer.Instance.SendToALL(pkg);
        }

        protected void HandleRSAKey(GSPacketIn packet)
        {
            RSAParameters para = default(RSAParameters);
            para.Modulus = packet.ReadBytes(128);
            para.Exponent = packet.ReadBytes();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(para);
            this.SendKeyAndName();
        }

        private void SendKeyAndName()
        {
            GSPacketIn pkg = new GSPacketIn(1);
            pkg.WriteString(AppConfig.AppSettings["CrossServerKey"]);
            pkg.WriteString(AppConfig.AppSettings["ServerName"]);
            this.SendTCP(pkg);
        }
        public void SendPacket(GSPacketIn packet)
        {
            this.SendTCP(packet);
        }

        public CrossServerConnector(string ip, int port, string key, byte[] readBuffer, byte[] sendBuffer) : base(ip, port, true, readBuffer, sendBuffer)
		{
			this.key = key;
			base.Strict = false;
		}
	}
}
