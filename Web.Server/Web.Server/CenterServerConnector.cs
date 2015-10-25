using Game.Base;
using Game.Base.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Web.Server
{
    public class CenterServerConnector : BaseConnector
    {
        public CenterServerConnector(string ip, int port, byte[] readBuffer, byte[] sendBuffer) : base(ip, port, false, readBuffer, sendBuffer)
		{
            base.Strict = false;
        }
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

        private void AsynProcessPacket(object state)
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
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                WebServer.log.Error("AsynProcessPacket", ex);
            }
        }

        private void HandleRSAKey(GSPacketIn pkg)
        {
            RSAParameters para = default(RSAParameters);
            para.Modulus = pkg.ReadBytes(128);
            para.Exponent = pkg.ReadBytes();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(para);
            this.SendRSALogin(rsa, "a3sdfi792kkliasfasdfshu290l-z:)(*");
        }
        public void SendRSALogin(RSACryptoServiceProvider rsa, string key)
        {
            GSPacketIn pkg = new GSPacketIn(1);
            pkg.Write(rsa.Encrypt(Encoding.UTF8.GetBytes(key), false));
            this.SendTCP(pkg);
        }


    }
}
