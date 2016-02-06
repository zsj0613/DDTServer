using Game.Base;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Lsj.Util.Logs;
using System;
using System.Reflection;
using System.Text;
using System.Threading;
namespace Game.Server
{
	public class GameClient : BaseClient
	{
		private static LogProvider log => LogProvider.Default;
		private static readonly byte[] POLICY = Encoding.UTF8.GetBytes("<?xml version=\"1.0\"?><!DOCTYPE cross-domain-policy SYSTEM \"http://www.adobe.com/xml/dtds/cross-domain-policy.dtd\"><cross-domain-policy><allow-access-from domain=\"*\" to-ports=\"*\" /></cross-domain-policy>\0");
		public static readonly string MANAGER_KEY = "kutlizlii8so0023k987sdf()*";
		protected GamePlayer m_player;
		public int Version;
		protected long m_pingTime = DateTime.Now.Ticks;
		protected IPacketLib m_packetLib;
		protected PacketProcessor m_packetProcessor;
		protected GameServer _srvr;
		public GamePlayer Player
		{
			get
			{
				return this.m_player;
			}
			set
			{
				GamePlayer oldPlayer = Interlocked.Exchange<GamePlayer>(ref this.m_player, value);
				if (oldPlayer != null)
				{
					oldPlayer.Quit();
				}
			}
		}
		public long PingTime
		{
			get
			{
				return this.m_pingTime;
			}
		}
		public IPacketLib Out
		{
			get
			{
				return this.m_packetLib;
			}
			set
			{
				this.m_packetLib = value;
			}
		}
		public PacketProcessor PacketProcessor
		{
			get
			{
				return this.m_packetProcessor;
			}
		}
		public GameServer Server
		{
			get
			{
				return this._srvr;
			}
		}
		public override void OnRecv(int num_bytes)
		{
			if (this.m_packetProcessor != null)
			{
				base.OnRecv(num_bytes);
			}
			else
			{
				if (this.m_readBuffer[0] == 60)
				{
					this.m_sock.Send(GameClient.POLICY);
				}
				else
				{
					base.OnRecv(num_bytes);
				}
			}
			this.m_pingTime = DateTime.Now.Ticks;
		}
		public override void OnRecvPacket(GSPacketIn pkg)
		{
			if (this.m_packetProcessor == null)
			{
				this.m_packetLib = AbstractPacketLib.CreatePacketLibForVersion(1, this);
				this.m_packetProcessor = new PacketProcessor(this);
			}
			if (this.m_player != null)
			{
				pkg.ClientID = this.m_player.PlayerId;
				pkg.WriteHeader();
			}
			pkg.ClearChecksum();
			switch (pkg.Code)
			{
			case 254:
				this.HandleManagerCmd(pkg);
				break;
			case 255:
				this.HandleManagerLogin(pkg);
				break;
			default:
				this.m_packetProcessor.HandlePacket(pkg);
				break;
			}
		}
		public void HandleManagerLogin(GSPacketIn packet)
		{
			string key = packet.ReadString();
			if (key == GameClient.MANAGER_KEY)
			{
				GameServer.ManagerLogined = true;
			}
			else
			{
				this.Disconnect();
			}
		}
		public void HandleManagerCmd(GSPacketIn packet)
		{
			if (GameServer.ManagerLogined)
			{
				string cmd = packet.ReadString();
				try
				{
					if (!CommandMgr.HandleCommandNoPlvl(this, cmd))
					{
						this.DisplayMessage("Unknown command: " + cmd);
					}
				}
				catch (Exception ex)
				{
					this.DisplayMessage("error:" + ex.Message);
				}
			}
		}
		public override void SendTCP(GSPacketIn pkg)
		{
			base.SendTCP(pkg);
		}
		public override void DisplayMessage(string msg)
		{
			if (GameServer.ManagerLogined)
			{
				GSPacketIn pkg = new GSPacketIn(253);
				pkg.WriteString(msg);
				this.SendTCP(pkg);
			}
			else
			{
				base.DisplayMessage(msg);
			}
		}
		protected override void OnConnect()
		{
			base.OnConnect();
			this.m_pingTime = DateTime.Now.Ticks;
		}
		public override void Disconnect()
		{
			base.Disconnect();
		}
		protected override void OnDisconnect()
		{
			try
			{
				GamePlayer player = Interlocked.Exchange<GamePlayer>(ref this.m_player, null);
				if (player != null)
				{
					player.ClearFightBag();
					LoginMgr.ClearLoginPlayer(player.PlayerCharacter.ID, this);
					player.Quit();
				}
				byte[] temp = this.m_sendBuffer;
				this.m_sendBuffer = null;
				this._srvr.ReleasePacketBuffer(temp);
				temp = this.m_readBuffer;
				this.m_readBuffer = null;
				this._srvr.ReleasePacketBuffer(temp);
				base.OnDisconnect();
			}
			catch (Exception e)
			{
				//if (GameClient.log.IsErrorEnabled)
				{
					GameClient.log.Error("OnDisconnect", e);
				}
			}
		}
		public void SavePlayer()
		{
			try
			{
				if (this.m_player != null)
				{
					this.m_player.SaveIntoDatabase();
				}
			}
			catch (Exception e)
			{
				//if (GameClient.log.IsErrorEnabled)
				{
					GameClient.log.Error("SavePlayer", e);
				}
			}
		}
		public GameClient(GameServer svr, byte[] read, byte[] send) : base(read, send)
		{
			this._srvr = svr;
			this.m_player = null;
			base.Encryted = true;
			base.AsyncPostSend = true;
		}
		public override string ToString()
		{
			return new StringBuilder(128).Append(" pakLib:").Append((this.Out == null) ? "(null)" : this.Out.GetType().FullName).Append(" IP:").Append(base.TcpEndpoint).Append(" char:").Append((this.Player == null) ? "null" : this.Player.PlayerCharacter.NickName).ToString();
		}
	}
}
