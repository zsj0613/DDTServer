using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Games;
using System;
using System.Collections.Generic;
namespace Game.Server.Rooms
{
	public class StartGameAction : IAction
	{
		private BaseRoom m_room;
		public StartGameAction(BaseRoom room)
		{
			this.m_room = room;
		}
		public void Execute()
		{
			if (this.m_room.CanStart())
			{
				List<GamePlayer> players = this.m_room.GetPlayers();
				if (this.m_room.RoomType == eRoomType.Freedom)
				{
					List<IGamePlayer> red = new List<IGamePlayer>();
					List<IGamePlayer> blue = new List<IGamePlayer>();
					foreach (GamePlayer p in players)
					{
						if (p != null)
						{
							if (p.CurrentRoomTeam == 1)
							{
								red.Add(p);
							}
							else
							{
								blue.Add(p);
							}
						}
					}
					BaseGame game = GameMgr.StartPVPGame(this.m_room.RoomId, red, blue, this.m_room.MapId, this.m_room.RoomType, this.m_room.GameType, (int)this.m_room.TimeMode);
					this.StartGame(game);
				}
				else
				{
                    if (this.m_room.RoomType == eRoomType.Exploration || this.m_room.RoomType == eRoomType.Boss || this.m_room.RoomType == eRoomType.Treasure || this.m_room.RoomType == eRoomType.Training || this.m_room.RoomType == eRoomType.FightLab || this.m_room.RoomType == eRoomType.FightNPC || this.m_room.RoomType == eRoomType.WorldBoss) 
					{
						List<IGamePlayer> matchPlayers = new List<IGamePlayer>();
						foreach (GamePlayer p in players)
						{
							if (p != null)
							{
								matchPlayers.Add(p);
							}
						}
						this.m_room.UpdatePveRoomTimeMode();
						BaseGame game = GameMgr.StartPVEGame(this.m_room.RoomId, matchPlayers, this.m_room.MapId, this.m_room.RoomType, this.m_room.GameType, (int)this.m_room.TimeMode, this.m_room.HardLevel, this.m_room.LevelLimits);
						this.StartGame(game);
					}
					else
					{
						if (this.m_room.RoomType == eRoomType.Match)
						{
							this.m_room.UpdateAvgLevel();
							BattleServer server = BattleMgr.AddRoom(this.m_room);
							if (server != null)
							{
								this.m_room.BattleServer = server;
								this.m_room.IsPlaying = true;
								this.m_room.SendStartPickUp();
							}
							else
							{
								this.m_room.SendCancelPickUp();
							}
						}
					}
				}
				RoomMgr.WaitingRoom.SendUpdateRoom(this.m_room);
			}
			else
			{
				this.m_room.SendPlayerState();
			}
		}
		private void StartGame(BaseGame game)
		{
			if (game != null)
			{
				this.m_room.IsPlaying = true;
				this.m_room.StartGame(game);
			}
			else
			{
				this.m_room.IsPlaying = false;
				this.m_room.SendPlayerState();
			}
		}
	}
}
