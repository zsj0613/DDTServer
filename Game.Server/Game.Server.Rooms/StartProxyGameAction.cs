using Game.Server.Battle;
using System;
namespace Game.Server.Rooms
{
	public class StartProxyGameAction : IAction
	{
		private BaseRoom m_room;
		private ProxyGame m_game;
		public StartProxyGameAction(BaseRoom room, ProxyGame game)
		{
			this.m_room = room;
			this.m_game = game;
		}
		public void Execute()
		{
			this.m_room.StartGame(this.m_game);
		}
	}
}
