using Bussiness;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using System;
using System.Collections.Generic;
using Game.Language;
namespace Game.Server.Rooms
{
	internal class EnterRoomAction : IAction
	{
		private GamePlayer m_player;
		private int m_roomId;
		private string m_pwd;
		private int m_type;
		private int m_hallType;
		private bool m_isInvite;
		public EnterRoomAction(GamePlayer player, int roomId, string pwd, int type, int hallType, bool isInvite)
		{
			this.m_player = player;
			this.m_roomId = roomId;
			this.m_pwd = pwd;
			this.m_type = type;
			this.m_hallType = hallType;
			this.m_isInvite = isInvite;
		}
		public void Execute()
		{
			bool result = true;
			if (this.m_player.IsActive)
			{
				if (this.m_player.CurrentRoom != null)
				{
					this.m_player.CurrentRoom.RemovePlayerUnsafe(this.m_player);
				}
				BaseRoom[] rooms = RoomMgr.Rooms;
				BaseRoom rm;
				if (this.m_roomId == -1)
				{
					rm = this.FindRandomRoom(rooms);
					if (rm == null)
					{
						this.m_player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("EnterRoomAction.noroom", new object[0]));
						this.m_player.Out.SendRoomLoginResult(false);
						List<BaseRoom> list = RoomMgr.GetWaitingRoom(this.m_hallType, 0, 9, 0);
						this.m_player.Out.SendUpdateRoomList(list);
						return;
					}
				}
				else
				{
					if (this.m_roomId > rooms.Length || this.m_roomId <= 0)
					{
						this.m_player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("EnterRoomAction.noexist", new object[0]));
						return;
					}
					rm = rooms[this.m_roomId - 1];
				}
				if (!rm.IsUsing)
				{
					this.m_player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("EnterRoomAction.noexist", new object[0]));
					List<BaseRoom> list = RoomMgr.GetWaitingRoom(this.m_hallType, 0, 9, 0);
					this.m_player.Out.SendUpdateRoomList(list);
				}
				else
				{
					if (this.m_hallType == 1 && (rm.RoomType == eRoomType.Boss || rm.RoomType == eRoomType.Exploration || rm.RoomType == eRoomType.Treasure))
					{
						this.m_player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("EnterRoomAction.NotInPve", new object[0]));
					}
					else
					{
						if (this.m_hallType == 2 && (rm.RoomType == eRoomType.Freedom || rm.RoomType == eRoomType.Match))
						{
							this.m_player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("EnterRoomAction.NotInPvp", new object[0]));
						}
						else
						{
							if (rm.IsPlaying)
							{
								if (rm.Game is PVEGame)
								{
									PVEGame pveGame = rm.Game as PVEGame;
									if (pveGame.GameState != eGameState.SessionPrepared || !this.m_isInvite)
									{
										this.m_player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("EnterRoomAction.start", new object[0]));
										result = false;
									}
								}
								else
								{
									this.m_player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("EnterRoomAction.start", new object[0]));
									result = false;
								}
							}
							if (result)
							{
								if (rm.PlayerCount == rm.PlacesCount)
								{
									result = false;
									this.m_player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("EnterRoomAction.full", new object[0]));
								}
								else
								{
									if (!rm.NeedPassword || rm.Password == this.m_pwd)
									{
										if (rm.Game == null || rm.Game.CanAddPlayer())
										{
											if (rm.RoomType == eRoomType.Exploration && rm.LevelLimits > (int)rm.GetLevelLimit(this.m_player))
											{
												this.m_player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("EnterRoomAction.level", new object[0]));
												return;
											}
											RoomMgr.WaitingRoom.RemovePlayer(this.m_player);
											this.m_player.Out.SendRoomLoginResult(true);
											this.m_player.Out.SendRoomCreate(rm);
											if (rm.AddPlayerUnsafe(this.m_player))
											{
												if (rm.Game != null)
												{
													rm.Game.AddPlayer(this.m_player);
												}
											}
											RoomMgr.WaitingRoom.SendUpdateRoom(rm);
											this.m_player.Out.SendRoomChange(rm);
										}
									}
									else
									{
										string msg;
										if (rm.NeedPassword && string.IsNullOrEmpty(this.m_pwd))
										{
											msg = LanguageMgr.GetTranslation("EnterRoomAction.EnterPassword", new object[0]);
										}
										else
										{
											msg = LanguageMgr.GetTranslation("EnterRoomAction.passworderror", new object[0]);
										}
										this.m_player.Out.SendMessage(eMessageType.ERROR, msg);
										this.m_player.Out.SendRoomLoginResult(false);
									}
								}
							}
							if (!result)
							{
								if (this.m_roomId != -1)
								{
									List<BaseRoom> list = RoomMgr.GetWaitingRoom(this.m_hallType, 0, 8, this.m_roomId);
									list.Add(rooms[this.m_roomId - 1]);
									this.m_player.Out.SendUpdateRoomList(list);
								}
							}
						}
					}
				}
			}
		}
		private BaseRoom FindRandomRoom(BaseRoom[] rooms)
		{
			BaseRoom result;
			for (int i = 0; i < rooms.Length; i++)
			{
				if (rooms[i].PlayerCount > 0 && rooms[i].CanAddPlayer() && !rooms[i].NeedPassword && !rooms[i].IsPlaying && rooms[i].RoomType != eRoomType.Training)
				{
					if (2 != this.m_type)
					{
						if (rooms[i].RoomType == (eRoomType)this.m_type)
						{
							result = rooms[i];
							return result;
						}
					}
					else
					{
						if (rooms[i].RoomType == (eRoomType)this.m_type && rooms[i].LevelLimits < (int)rooms[i].GetLevelLimit(this.m_player))
						{
							result = rooms[i];
							return result;
						}
					}
				}
			}
			result = null;
			return result;
		}
	}
}
