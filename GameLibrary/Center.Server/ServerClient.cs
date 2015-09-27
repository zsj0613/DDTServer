using Bussiness;
using Center.Server.Managers;
using Game.Base;
using Game.Base.Packets;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;

namespace Center.Server
{
    public class ServerClient : BaseClient
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private RSACryptoServiceProvider _rsa;
        private CenterServer _svr;
        private BaseClient m_currentCmdClient = null;
        public bool IsManager = false;
        public bool NeedSyncMacroDrop = false;
        public static readonly string MANAGER_KEY = "a3sdfi792kklishu290l-z:)(*";
        public ServerInfo Info
        {
            get;
            set;
        }
        protected override void OnConnect()
        {
            base.OnConnect();
            this._rsa = new RSACryptoServiceProvider();
            RSAParameters para = this._rsa.ExportParameters(false);
            this.SendRSAKey(para.Modulus, para.Exponent);
        }
        protected override void OnDisconnect()
        {
            base.OnDisconnect();
            this._rsa = null;
            if (!this.IsManager)
            {
                List<Player> player = LoginMgr.GetServerPlayers(this);
                LoginMgr.RemovePlayer(player);
                this.SendUserOffline(player);
                ServerClient.log.InfoFormat("{0}({1}:{2}) disconntected!", this.Info.Name, this.Info.IP, this.Info.Port);
                if (this.Info != null)
                {
                    this.Info.State = 1;
                    this.Info.Online = 0;
                }
            }
        }
        public override void OnRecvPacket(GSPacketIn pkg)
        {
            short code = pkg.Code;
            if (code <= 130)
            {
                if (code <= 72)
                {
                    switch (code)
                    {
                        case 1:
                            this.HandleLogin(pkg);
                            break;
                        case 2:
                        case 7:
                        case 8:
                        case 9:
                        case 16:
                        case 17:
                        case 18:
                        case 20:
                        case 21:
                        case 22:
                        case 23:
                        case 24:
                            break;
                        case 3:
                            this.HandleUserLogin(pkg);
                            break;
                        case 4:
                            this.HandleUserOffline(pkg);
                            break;
                        case 5:
                            this.HandleUserOnline(pkg);
                            break;
                        case 6:
                            this.HandleQuestUserState(pkg);
                            break;
                        case 10:
                            this.HandkeItemStrengthen(pkg);
                            break;
                        case 11:
                            this.HandleCmdResult(pkg);
                            break;
                        case 12:
                            this.HandlePing(pkg);
                            break;
                        case 13:
                            this.HandleUpdatePlayerState(pkg);
                            break;
                        case 14:
                            this.HandleGetItemMessage(pkg);
                            break;
                        case 15:
                            this.HandleShutdown(pkg);
                            break;
                        case 19:
                            this.HandleChatScene(pkg);
                            break;
                        case 26:
                            this.HandleMarryRoomInfoToPlayer(pkg);
                            break;
                        default:
                            if (code != 37)
                            {
                                if (code == 72)
                                {
                                    this.HandleBigBugle(pkg);
                                }
                            }
                            else
                            {
                                this.HandleChatPersonal(pkg);
                            }
                            break;
                    }
                }
                else
                {
                    switch (code)
                    {
                        case 117:
                            this.HandleMailResponse(pkg);
                            break;
                        case 118:
                            this.HandleMessage(pkg);
                            break;
                        default:
                            if (code != 123)
                            {
                                switch (code)
                                {
                                    case 128:
                                        this.HandleConsortiaResponse(pkg);
                                        break;
                                    case 130:
                                        this.HandleConsortiaCreate(pkg);
                                        break;
                                }
                            }
                            else
                            {
                                this.HandleDispatches(pkg);
                            }
                            break;
                    }
                }
            }
            else
            {
                if (code <= 178)
                {
                    switch (code)
                    {
                        case 156:
                            this.HandleConsortiaOffer(pkg);
                            break;
                        case 157:
                            break;
                        case 158:
                            this.HandleConsortiaFight(pkg);
                            break;
                        default:
                            switch (code)
                            {
                                case 165:
                                    this.HandleFriendState(pkg);
                                    break;
                                case 166:
                                    this.HandleFirendResponse(pkg);
                                    break;
                                case 167:
                                    break;
                                case 168:
                                    this.HandleUpdateLimitCount(pkg);
                                    break;
                                default:
                                    if (code == 178)
                                    {
                                        this.HandleMacroDrop(pkg);
                                    }
                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    if (code != 204)
                    {
                        switch (code)
                        {
                            case 240:
                                this.HandleIPAndPort(pkg);
                                break;
                            case 241:
                                this.HandleMarryRoomDispose(pkg);
                                break;
                            default:
                                switch (code)
                                {
                                    case 252:
                                        this.HandleManagerLogin(pkg);
                                        break;
                                    case 254:
                                        this.HandleManagerCmd(pkg);
                                        break;
                                }
                                break;
                        }
                    }
                    else
                    {
                        this.HandleUpdateShopNotice(pkg);
                    }
                }
            }
        }
        public void HandleManagerLogin(GSPacketIn pkg)
        {
            string key = pkg.ReadString();
            if (key == ServerClient.MANAGER_KEY)
            {
                this.IsManager = true;
            }
            else
            {
                this.Disconnect();
            }
        }
        public void HandleManagerCmd(GSPacketIn pkg)
        {

        }
        public override void DisplayMessage(string msg)
        {
            if (this.IsManager)
            {
                GSPacketIn pkg = new GSPacketIn(255);
                pkg.WriteString(msg);
                this.SendTCP(pkg);
            }
        }
        private void HandleGetItemMessage(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }
        public void HandleLogin(GSPacketIn pkg)
        {
            byte[] rgb = pkg.ReadBytes();
            string content = Encoding.UTF8.GetString(this._rsa.Decrypt(rgb, false));
            string[] temp = content.Split(new char[]
            {
                ','
            });
            if (temp.Length == 2)
            {
                this._rsa = null;
                int id = int.Parse(temp[0]);
                this.Info = ServerMgr.GetServerInfo(id);
                if (this.Info == null || this.Info.State != 1)
                {
                    ServerClient.log.ErrorFormat("Error Login Packet from {0} want to login serverid:{1}", base.TcpEndpoint, id);
                    this.Disconnect();
                }
                else
                {
                    base.Strict = false;
                    this.Info.Online = 0;
                    this.Info.State = 2;
                }
            }
            else
            {
                ServerClient.log.ErrorFormat("Error Login Packet from {0},content:{1}", base.TcpEndpoint, content);
                this.Disconnect();
            }
        }
        public void HandleIPAndPort(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }
        private void HandleUserLogin(GSPacketIn pkg)
        {
            int playerid = pkg.ReadInt();
            if (LoginMgr.TryLoginPlayer(playerid, this))
            {
                this.SendAllowUserLogin(playerid, true);
            }
            else
            {
                this.SendAllowUserLogin(playerid, false);
            }
        }
        private void HandleUserOnline(GSPacketIn pkg)
        {
            int count = pkg.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int playerid = pkg.ReadInt();
                int consortiaid = pkg.ReadInt();
                LoginMgr.PlayerLogined(playerid, this);
            }
            this._svr.SendToALL(pkg, this);
        }
        private void HandleUserOffline(GSPacketIn pkg)
        {
            List<int> users = new List<int>();
            int count = pkg.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int playerid = pkg.ReadInt();
                int consortiaid = pkg.ReadInt();
                LoginMgr.PlayerLoginOut(playerid, this);
            }
            this._svr.SendToALL(pkg);
        }
        private void HandleUserPrivateMsg(GSPacketIn pkg, int playerid)
        {
            ServerClient client = LoginMgr.GetServerClient(playerid);
            if (client != null)
            {
                client.SendTCP(pkg);
            }
        }
        public void HandleUserPublicMsg(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }
        public void HandleQuestUserState(GSPacketIn pkg)
        {
            int playerid = pkg.ReadInt();
            ServerClient client = LoginMgr.GetServerClient(playerid);
            if (client == null)
            {
                this.SendUserState(playerid, false);
            }
            else
            {
                this.SendUserState(playerid, true);
            }
        }
        public void HandlePing(GSPacketIn pkg)
        {
            this.Info.Online = pkg.ReadInt();
            this.Info.State = ServerMgr.GetState(this.Info.Online, this.Info.Total);
        }
        public void HandleChatPersonal(GSPacketIn pkg)
        {
            int playerid = pkg.ReadInt();
            ServerClient client = LoginMgr.GetServerClient(playerid);
            if (client != null)
            {
                client.SendTCP(pkg);
            }
            else
            {
                int id = pkg.ClientID;
                string nickName = pkg.ReadString();
                GSPacketIn packet = new GSPacketIn(38);
                packet.WriteInt(1);
                packet.WriteInt(id);
                packet.WriteString(nickName);
                this.SendTCP(packet);
            }
        }
        public void HandleBigBugle(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }
        public void HandleDispatches(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }
        //private void HandleAreaBigBugle(GSPacketIn pkg)
        //{
        //    ClientBinding vBinding = new ClientBinding();
        //    vBinding.Address = CenterServer.Instance.Config.BigBugleServerIp;
        //    using (ChannelFactory<IService> Factory = new ChannelFactory<IService>(vBinding.Binding, vBinding.Uri))
        //    {
        //        IService server = Factory.CreateChannel();
        //        using (server as IDisposable)
        //        {
        //            int userid = pkg.ReadInt();
        //            int areaid = pkg.ReadInt();
        //            string nickName = pkg.ReadString();
        //            string message = pkg.ReadString();
        //            try
        //            {
        //                if (!server.SendAreaBigBugle(userid, areaid, nickName, message, CenterServer.Instance.Config.BigAreaID))
        //                {
        //                    using (ProduceBusiness db = new ProduceBusiness())
        //                    {
        //                        db.AddAreaBigBugleRecord(userid, areaid, nickName, message, "127.0.0.1");
        //                    }
        //                }
        //            }
        //            catch (Exception e_BB)
        //            {
        //                using (ProduceBusiness db = new ProduceBusiness())
        //                {
        //                    db.AddAreaBigBugleRecord(userid, areaid, nickName, message, "127.0.0.1");
        //                }
        //            }
        //        }
        //    }
        //}
        public void SendAreaBigBugleToServer(GSPacketIn pkg)
        {
            this.SendTCP(pkg);
        }
        public void HandleFriendState(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }
        public void HandleUpdateLimitCount(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }
        public void HandleUpdateShopNotice(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }
        public void HandleFirendResponse(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }
        public void HandleMailResponse(GSPacketIn pkg)
        {
            int playerid = pkg.ReadInt();
            this.HandleUserPrivateMsg(pkg, playerid);
        }
        public void HandleChatScene(GSPacketIn pkg)
        {
            int areaid = pkg.ReadInt();
            byte channel = pkg.ReadByte();
            byte b = channel;
            if (b == 3)
            {
                this.HandleChatConsortia(pkg);
            }
        }
        public void HandleChatConsortia(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }
        public void HandleConsortiaResponse(GSPacketIn pkg)
        {

            this._svr.SendToALL(pkg, null);
        }
        public void HandleConsortiaOffer(GSPacketIn pkg)
        {
            int id = pkg.ReadInt();
            int offer = pkg.ReadInt();
            int riches = pkg.ReadInt();
        }
        public void HandleConsortiaCreate(GSPacketIn pkg)
        {
            int id = pkg.ReadInt();
            int offer = pkg.ReadInt();
            this._svr.SendToALL(pkg, null);
        }
        public void HandleConsortiaUpGrade(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }
        public void HandleConsortiaFight(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg);
        }
        public void HandkeItemStrengthen(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg, this);
        }
        public void HandleUpdatePlayerState(GSPacketIn pkg)
        {
            int playerId = pkg.ReadInt();
            Player player = LoginMgr.GetPlayer(playerId);
            if (player != null && player.CurrentServer != null)
            {
                player.CurrentServer.SendTCP(pkg);
            }
        }
        public void HandleMarryRoomInfoToPlayer(GSPacketIn pkg)
        {
            int playerId = pkg.ReadInt();
            Player player = LoginMgr.GetPlayer(playerId);
            if (player != null && player.CurrentServer != null)
            {
                player.CurrentServer.SendTCP(pkg);
            }
        }
        public void HandleMarryRoomDispose(GSPacketIn pkg)
        {
            this._svr.SendToALL(pkg);
        }
        public void HandleShutdown(GSPacketIn pkg)
        {
            int serverID = pkg.ReadInt();
            bool isStoping = pkg.ReadBoolean();
            if (isStoping)
            {
                Console.WriteLine(serverID + "  begin stoping !");
            }
            else
            {
                Console.WriteLine(serverID + "  is stoped !");
            }
        }
        public void HandleMacroDrop(GSPacketIn pkg)
        {
            Dictionary<int, int> temp = new Dictionary<int, int>();
            int count = pkg.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int templateId = pkg.ReadInt();
                int dropCount = pkg.ReadInt();
                temp.Add(templateId, dropCount);
            }
            MacroDropMgr.DropNotice(temp);
            this.NeedSyncMacroDrop = true;
        }
        public void SendRSAKey(byte[] m, byte[] e)
        {
            GSPacketIn pkg = new GSPacketIn(0);
            pkg.Write(m);
            pkg.Write(e);
            this.SendTCP(pkg);
        }
        public void SendAllowUserLogin(int playerid, bool allow)
        {
            GSPacketIn pkg = new GSPacketIn(3);
            pkg.WriteInt(playerid);
            pkg.WriteBoolean(allow);
            this.SendTCP(pkg);
        }
        public void SendKitoffUser(int playerid)
        {
            this.SendKitoffUser(playerid, Game.Language.LanguageMgr.GetTranslation("Center.Server.SendKitoffUser", new object[0]));
        }
        public void SendKitoffUser(int playerid, string msg)
        {
            GSPacketIn pkg = new GSPacketIn(2);
            pkg.WriteInt(playerid);
            pkg.WriteString(msg);
            this.SendTCP(pkg);
        }
        public void SendUserOffline(List<Player> users)
        {
            for (int i = 0; i < users.Count; i += 100)
            {
                int count = (i + 100 > users.Count) ? (users.Count - i) : 100;
                GSPacketIn pkg = new GSPacketIn(4);
                pkg.WriteInt(count);
                for (int j = i; j < i + count; j++)
                {
                    pkg.WriteInt(users[j].Id);
                    pkg.WriteInt(0);
                }
                this.SendTCP(pkg);
                this._svr.SendToALL(pkg, this);
            }
        }
        public void SendUserState(int player, bool state)
        {
            GSPacketIn pkg = new GSPacketIn(6, player);
            pkg.WriteBoolean(state);
            this.SendTCP(pkg);
        }
        public void SendChargeMoney(int player, string chargeID)
        {
            GSPacketIn pkg = new GSPacketIn(9, player);
            pkg.WriteString(chargeID);
            this.SendTCP(pkg);
        }
        public void SendChargeGiftToken(int player, int giftToken)
        {
            GSPacketIn pkg = new GSPacketIn(16, player);
            pkg.WriteInt(giftToken);
            this.SendTCP(pkg);
        }
        public void SendASS(bool state)
        {
            GSPacketIn pkg = new GSPacketIn(7);
            pkg.WriteBoolean(state);
            this.SendTCP(pkg);
        }
        public void HandleCmdResult(GSPacketIn pkg)
        {
            if (this.m_currentCmdClient != null)
            {
                string result = string.Format("{0}:'{1}' execute:{2}", this.Info.Name, pkg.ReadString(), pkg.ReadBoolean());
                this.m_currentCmdClient.DisplayMessage(result);
                this.m_currentCmdClient = null;
            }
        }
        public void SendCmd(BaseClient cmdclient, string cmdLine)
        {
            this.m_currentCmdClient = cmdclient;
            if (cmdLine.Length > 0 && cmdLine[0] == '/')
            {
                cmdLine = cmdLine.Remove(0, 1);
                cmdLine = cmdLine.Insert(0, "&");
            }
            GSPacketIn pkg = new GSPacketIn(11);
            pkg.WriteString(cmdLine);
            this.SendTCP(pkg);
        }
        public ServerClient(CenterServer svr) : base(new byte[2048], new byte[2048])
        {
            this._svr = svr;
        }
        public void HandleMessage(GSPacketIn packet)
        {
            if (this.m_currentCmdClient != null)
            {
                this.m_currentCmdClient.DisplayMessage(string.Format("{0}:{1}", this.Info.Name, packet.ReadString()));
            }
        }
    }
}
