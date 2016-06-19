using Bussiness;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.SpaRooms;
using Lsj.Util.Logs;
using SqlDataProvider.Data;
using System;
using System.Linq;
using System.Reflection;

namespace Game.Server.Packets.Client
{
	[PacketHandler(175, "创建温泉房间")]
	public class SpaRoomCreateHandler : AbstractPlayerPacketHandler
	{
		protected static LogProvider log => LogProvider.Default;
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result;
			if (player.PlayerCharacter.HasBagPassword && player.PlayerCharacter.IsLocked)
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				result = 0;
			}
			else
			{
				if (player.CurrentRoom != null)
				{
					player.CurrentRoom.RemovePlayerUnsafe(player);
				}
				if (player.CurrentSpaRoom != null)
				{
					player.CurrentSpaRoom.RemovePlayer(player);
				}
				SpaRoom[] rooms = SpaRoomMgr.GetAllSpaRoom();
				if (rooms != null)
				{
					if (rooms.Count<SpaRoom>() >2000)
					{
						result = 0;
						return result;
					}
				}
				SpaRoomInfo info = new SpaRoomInfo();
				info.RoomName = packet.ReadString().Replace(";", "");
				info.Pwd = packet.ReadString();
				info.RoomIntroduction = packet.ReadString();
				info.MaxCount = packet.ReadInt();
				info.ServerID = GameServer.Instance.Config.ServerID;
				info.AvailTime = SpaRoomMgr.priRoomInit_MinLimit;
				info.RoomType = 3;
				string[] array = new string[]
				{
					"1",
					"2"
				};
				string[] money = "800,1600".Split(new char[]
				{
					','
				});
				if (money.Length < 2)
				{
					
						SpaRoomCreateHandler.log.Error("SpaRoomCreateMoney node in configuration file is wrong");
					
					result = 0;
				}
				else
				{
					int needMoney;
					if (info.MaxCount == 4)
					{
						needMoney = int.Parse(money[0]);
					}
					else
					{
						if (info.MaxCount == 8)
						{
							needMoney = int.Parse(money[1]);
						}
						else
						{
							needMoney = int.Parse(money[1]);
							info.MaxCount = 8;
						}
					}
					if (player.PlayerCharacter.Money >= needMoney)
					{
						SpaRoom room = SpaRoomMgr.CreateSpaRoom(player, info);
						if (room != null)
						{
							player.RemoveMoney(needMoney, LogMoneyType.Spa, LogMoneyType.Spa_Room_Creat);
							player.Out.SendSpaRoomInfo(player, room);
							player.Out.SendSpaRoomLogin(player);
							room.SendSpaRoomInfoUpdateToSpaScenePlayers(room);
							CountBussiness.InsertSystemPayCount(player.PlayerCharacter.ID, needMoney, 0, 3, 6);
						}
						result = 0;
					}
					else
					{
						player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("UserFirecrackersCommand.MoneyNotEnough", new object[0]));
						result = 1;
					}
				}
			}
			return result;
		}
	}
}
