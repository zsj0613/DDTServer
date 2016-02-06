using Game.Base;
using Game.Base.Packets;
using Game.Logic;
using System;
namespace Game.Server.Battle
{
	public class ProxyGame : AbstractGame
	{
		private FightServerConnector m_fightingServer;
		public ProxyGame(int id, FightServerConnector fightServer, eRoomType roomType, eGameType gameType, int timeType) : base(id, roomType, gameType, timeType)
		{
			this.m_fightingServer = fightServer;
			this.m_fightingServer.Disconnected += new ClientEventHandle(this.m_fightingServer_Disconnected);
		}
		public override void SendToAll(GSPacketIn pkg, IGamePlayer except)
		{
			this.m_fightingServer.SendToGameAllPlayer(base.Id, except, pkg);
		}
		private void m_fightingServer_Disconnected(BaseClient client)
		{
			this.Stop();
		}
		public override void ProcessData(GSPacketIn pkg)
		{
			this.m_fightingServer.SendToGame(base.Id, pkg);
		}
	}
}
