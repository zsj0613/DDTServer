using Bussiness;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.SceneMarryRooms;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Reflection;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(241, "礼堂创建")]
	public class MarryRoomCreateHandler : AbstractPlayerPacketHandler
	{
		protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result;
			if (!player.PlayerCharacter.IsMarried)
			{
				result = 1;
			}
			else
			{
				if (player.PlayerCharacter.IsCreatedMarryRoom)
				{
					result = 1;
				}
				else
				{
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
						if (player.CurrentMarryRoom != null)
						{
							player.CurrentMarryRoom.RemovePlayer(player);
						}
						MarryRoomInfo info = new MarryRoomInfo();
						info.Name = packet.ReadString().Replace(";", "");
						info.Pwd = packet.ReadString();
						info.MapIndex = packet.ReadInt();
						info.AvailTime = packet.ReadInt();
						info.MaxCount = packet.ReadInt();
						info.GuestInvite = packet.ReadBoolean();
						info.RoomIntroduction = packet.ReadString();
						info.ServerID = GameServer.Instance.Configuration.ServerID;
						info.IsHymeneal = false;
						string[] array = new string[]
						{
							"1",
							"2",
							"3"
						};
						string[] money = GameProperties.PRICE_MARRY_ROOM.Split(new char[]
						{
							','
						});
						if (money.Length < 3)
						{
							if (MarryRoomCreateHandler.log.IsErrorEnabled)
							{
								MarryRoomCreateHandler.log.Error("MarryRoomCreateMoney node in configuration file is wrong");
							}
							result = 1;
						}
						else
						{
							int needMoney;
							switch (info.AvailTime)
							{
							case 2:
								needMoney = int.Parse(money[0]);
								break;
							case 3:
								needMoney = int.Parse(money[1]);
								break;
							case 4:
								needMoney = int.Parse(money[2]);
								break;
							default:
								needMoney = int.Parse(money[2]);
								info.AvailTime = 4;
								break;
							}
							if (player.PlayerCharacter.Money >= needMoney)
							{
								MarryRoom room = MarryRoomMgr.CreateMarryRoom(player, info);
								if (room != null)
								{
									player.RemoveMoney(needMoney, LogMoneyType.Marry, LogMoneyType.Marry_Room);
									GSPacketIn pkg = player.Out.SendMarryRoomInfo(player, room);
									player.Out.SendMarryRoomLogin(player, true);
									room.SendToScenePlayer(pkg);
									CountBussiness.InsertSystemPayCount(player.PlayerCharacter.ID, needMoney, 0, 0, 0);
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
				}
			}
			return result;
		}
	}
}
