using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using Game.Server.Rooms;
using System;
using Game.Language;
using Bussiness.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(86, "游戏开始")]
	public class GameUserStartHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			BaseRoom room = player.CurrentRoom;
			int result;
			if (room != null && room.Host == player)
			{
				foreach (GamePlayer p in player.CurrentRoom.GetPlayers())
				{
					if (p.MainWeapon == null)
					{
						p.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip", new object[0]));
						result = 0;
						return result;
					}
				}
				if (room.RoomType == eRoomType.Treasure)
				{
					if (!player.IsPvePermission(room.MapId, room.HardLevel))
					{
						player.SendMessage(LanguageMgr.GetTranslation("GameUserStartHandler.level", new object[0]));
						result = 0;
						return result;
					}
				}
				else
				{
					if (room.RoomType == eRoomType.FightLab)
					{
						if (!player.IsFightLabPermission(room.MapId, room.HardLevel))
						{
							player.SendMessage(LanguageMgr.GetTranslation("GameUserStartHandler.level", new object[0]));
							result = 0;
							return result;
						}
					}
					else
					{
						if (room.RoomType == eRoomType.Boss)
						{
                            if (!player.IsPvePermission(room.MapId, room.HardLevel))
                            {
                                player.SendMessage(LanguageMgr.GetTranslation("GameUserStartHandler.level", new object[0]));
                                result = 0;
                                return result;
                            }
                            if (player.GetItemCount(88) <= 0)
                            {
                                player.SendMessage("你的副本通行证不足");
                                return 0;
                            }
                            else
                            {
                                player.PropBag.RemoveCountFromStack(player.PropBag.GetItemByTemplateID(0,88),1,eItemRemoveType.Use);
                            }
                        }
					}
				}
				RoomMgr.StartGame(player.CurrentRoom);
			}
			result = 0;
			return result;
		}
	}
}
