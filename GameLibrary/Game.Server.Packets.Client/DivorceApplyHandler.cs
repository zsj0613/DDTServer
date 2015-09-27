using Bussiness;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.SceneMarryRooms;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(248, "离婚")]
	internal class DivorceApplyHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			bool isInMovie = packet.ReadBoolean();
			int result;
			if (!player.PlayerCharacter.IsMarried)
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
					int needMoney = GameProperties.PRICE_DIVORCED;
					if (player.PlayerCharacter.Money < needMoney)
					{
						player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("DivorceApplyHandler.Msg1", new object[0]));
						result = 1;
					}
					else
					{
						if (player.PlayerCharacter.IsCreatedMarryRoom)
						{
							using (PlayerBussiness db = new PlayerBussiness())
							{
								db.DisposeMarryRoomInfo(player.PlayerCharacter.SelfMarryRoomID);
								GameServer.Instance.LoginServer.SendMarryRoomDisposeToPlayer(player.PlayerCharacter.SelfMarryRoomID);
							}
							MarryRoom[] rooms = MarryRoomMgr.GetAllMarryRoom();
							MarryRoom[] array = rooms;
							for (int i = 0; i < array.Length; i++)
							{
								MarryRoom room = array[i];
								if (room.Info.GroomID == player.PlayerCharacter.ID || room.Info.BrideID == player.PlayerCharacter.ID)
								{
									room.KillAllPlayer();
									GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(room.Info.GroomID);
									GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(room.Info.BrideID);
									GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(room.Info.GroomID, false, room.Info);
									GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(room.Info.BrideID, false, room.Info);
									MarryRoomMgr.RemoveMarryRoom(room);
									GSPacketIn pkg = new GSPacketIn(254);
									pkg.WriteInt(room.Info.ID);
									WorldMgr.MarryScene.SendToALL(pkg);
									room.StopTimer();
									if (isInMovie)
									{
										GSPacketIn pkg2 = new GSPacketIn(249);
										pkg2.WriteByte(9);
										room.SendToAll(pkg2);
										room.StopTimerForHymeneal();
										room.SendUserRemoveLate();
									}
									break;
								}
							}
						}
						player.RemoveMoney(needMoney, LogMoneyType.Marry, LogMoneyType.Marry_Unmarry);
						CountBussiness.InsertSystemPayCount(player.PlayerCharacter.ID, needMoney, 0, 0, 3);
						using (PlayerBussiness db = new PlayerBussiness())
						{
							PlayerInfo tempSpouse = db.GetUserSingleAllUserID(player.PlayerCharacter.SpouseID);
							if (tempSpouse == null || tempSpouse.Sex == player.PlayerCharacter.Sex)
							{
								result = 1;
								return result;
							}
							MarryApplyInfo info = new MarryApplyInfo();
							info.UserID = player.PlayerCharacter.SpouseID;
							info.ApplyUserID = player.PlayerCharacter.ID;
							info.ApplyUserName = player.PlayerCharacter.NickName;
							info.ApplyType = 3;
							info.LoveProclamation = "";
							info.ApplyResult = false;
							int id = 0;
							if (db.SavePlayerMarryNotice(info, 0, ref id))
							{
								GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(tempSpouse.ID);
								player.LoadMarryProp();
							}
						}
						player.QuestInventory.ClearMarryQuest();
						player.Out.SendPlayerDivorceApply(player, true, true);
						result = 0;
					}
				}
			}
			return result;
		}
	}
}
