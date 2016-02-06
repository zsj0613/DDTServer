using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;
using System.Collections.Generic;

namespace Game.Server.Packets.Client
{
	[PacketHandler(70, "邀请")]
	public class GameInviteHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result;
			if (player.CurrentRoom == null)
			{
				result = 0;
			}
			else
			{
				GSPacketIn pkg = packet.Clone();
				pkg.ClearContext();
				int id = packet.ReadInt();
				GamePlayer other = WorldMgr.GetPlayerById(id);
				if (other == player)
				{
					result = 0;
				}
				else
				{
					if (other == null)
					{
						player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("friendnotinthesameserver.Fail", new object[0]));
						result = 0;
					}
					else
					{
						int copyInviteLevelLimit = 1;
						if (other.PlayerCharacter.Grade < copyInviteLevelLimit && (player.CurrentRoom.RoomType == eRoomType.Treasure || player.CurrentRoom.RoomType == eRoomType.Boss || player.CurrentRoom.RoomType == eRoomType.Exploration))
						{
							player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("GameInviteHandler.LessThanLevelLimit", new object[]
							{
								copyInviteLevelLimit
							}));
							result = 0;
						}
						else
						{
							List<GamePlayer> players = player.CurrentRoom.GetPlayers();
							foreach (GamePlayer p in players)
							{
								if (p == other)
								{
									player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("friendnotinthesameserver.Sameroom", new object[0]));
									result = 0;
									return result;
								}
							}
							if (other != null && other.CurrentRoom == null)
							{
								pkg.WriteInt(player.PlayerCharacter.ID);
								pkg.WriteInt(player.CurrentRoom.RoomId);
								pkg.WriteInt(player.CurrentRoom.MapId);
								pkg.WriteByte(player.CurrentRoom.TimeMode);
								pkg.WriteByte((byte)player.CurrentRoom.RoomType);
								pkg.WriteByte((byte)player.CurrentRoom.HardLevel);
								pkg.WriteByte((byte)player.CurrentRoom.LevelLimits);
								pkg.WriteString(player.PlayerCharacter.NickName);
								pkg.WriteString(player.CurrentRoom.Name);
								pkg.WriteString(player.CurrentRoom.Password);
								if (player.CurrentRoom.RoomType == eRoomType.Treasure)
								{
									if (player.CurrentRoom.Game != null)
									{
										pkg.WriteInt((player.CurrentRoom.Game as PVEGame).SessionId);
									}
									else
									{
										pkg.WriteInt(0);
									}
								}
								else
								{
									pkg.WriteInt(-1);
								}
								other.Out.SendTCP(pkg);
							}
							else
							{
								if (other != null && other.CurrentRoom != null && other.CurrentRoom != player.CurrentRoom)
								{
									player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("friendnotinthesameserver.Room", new object[0]));
								}
							}
							result = 0;
						}
					}
				}
			}
			return result;
		}
	}
}
