using Game.Server.GameObjects;
using System;
namespace Game.Server.Rooms
{
	public class SwitchTeamAction : IAction
	{
		private GamePlayer m_player;
		public SwitchTeamAction(GamePlayer player)
		{
			this.m_player = player;
		}
		public void Execute()
		{
			BaseRoom room = this.m_player.CurrentRoom;
			if (room != null)
			{
				room.SwitchTeamUnsafe(this.m_player);
			}
		}
	}
}
