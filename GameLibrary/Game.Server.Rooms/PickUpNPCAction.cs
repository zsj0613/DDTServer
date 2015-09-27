using Game.Logic;
using Game.Server.GameObjects;
using System;
namespace Game.Server.Rooms
{
	public class PickUpNPCAction : IAction
	{
		private BaseRoom m_room;
		public PickUpNPCAction(BaseRoom room)
		{
			this.m_room = room;
		}
		public void Execute()
		{
			if (this.m_room.IsPlaying && this.m_room.Game == null)
			{
				this.m_room.IsPlaying = false;
				this.m_room.BattleServer = null;
				this.m_room.OldRoomType = this.m_room.RoomType;
				this.m_room.PickUpNPC = true;
				this.m_room.RoomType = eRoomType.Treasure;
				this.m_room.MapId = 105;
				this.m_room.GameType = eGameType.Treasure;
				foreach (GamePlayer p in this.m_room.GetPlayers())
				{
					p.Out.SendRoomChange(this.m_room);
				}
				RoomMgr.AddAction(new StartGameAction(this.m_room));
				this.m_room.Host.GameOver += new GamePlayer.PlayerGameOverEventHandle(this.Host_GameOver);
			}
		}
		private void Host_GameOver(AbstractGame game, bool isWin, int gainXp, bool isSpanArea, bool isCouple)
		{
			this.m_room.Host.GameOver -= new GamePlayer.PlayerGameOverEventHandle(this.Host_GameOver);
			if (isWin)
			{
				this.m_room.Host.Out.SendPickUpNPC();
			}
		}
	}
}
