using Game.Base;
using Game.Base.Packets;
using Lsj.Util.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Cross
{
    public class CenterServerClient : BaseClient
    {
        public string Name
        {
            private get;
            set;
        }
        private RSACryptoServiceProvider rsa;
        private CrossServer server;

        public CenterServerClient(CrossServer server) : base(new byte[2048], new byte[2048])
        {
            this.server = server;                     
        }
        protected override void OnConnect()
        {
            base.OnConnect();
            this.rsa = new RSACryptoServiceProvider();
            RSAParameters para = this.rsa.ExportParameters(false);
            this.SendRSAKey(para.Modulus, para.Exponent);
        }
        protected override void OnDisconnect()
        {
            base.OnDisconnect();
            this.rsa = null;
            if (this.Name != null)
            {
                CrossServer.log.Info(Name + " 已断开");
            }
        }
        public void SendRSAKey(byte[] m, byte[] e)
        {
            GSPacketIn pkg = new GSPacketIn(0);
            pkg.Write(m);
            pkg.Write(e);
            this.SendTCP(pkg);
        }
        public override void OnRecvPacket(GSPacketIn pkg)
        {
            short code = pkg.Code;
            switch (code)
            {
                case 1:
                    HandleLogin(pkg);
                    break;
                case 73:
                    HandleAreaBigBugle(pkg);
                    break;
                default:
                    break;
            }
        }

        private void HandleAreaBigBugle(GSPacketIn pkg)
        {
            var x = new GSPacketIn(25);
            x.WriteInt(pkg.ReadInt());
            x.WriteInt(pkg.ReadInt());
            x.WriteString(Name);
            x.WriteString(pkg.ReadString());
            x.WriteString(pkg.ReadString());
            foreach (var a in server.GetAllClients())
            {
                a.SendTCP(x);
            }
        }

        private void HandleLogin(GSPacketIn pkg)
        {
            var key = pkg.ReadString();
            if (key != AppConfig.AppSettings["Key"])
            {
                this.Disconnect();
                return;
            }
            this.Name = pkg.ReadString();
            CrossServer.log.Info(Name + " 已连接");            
        }
    }
}
