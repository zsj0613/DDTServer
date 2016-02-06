using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Linq;

namespace Game.Server.Packets.Client
{
	[PacketHandler(247, "求婚")]
	internal class MarryApplyHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result2;
			if (player.PlayerCharacter.IsMarried)
			{
				result2 = 1;
			}
			else
			{
				if (player.PlayerCharacter.HasBagPassword && player.PlayerCharacter.IsLocked)
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
					result2 = 1;
				}
				else
				{
					int SpouseID = packet.ReadInt();
					string loveProclamation = packet.ReadString();
					bool Broadcast = packet.ReadBoolean();
					bool result = false;
					bool removeRing = true;
					string SpouseName = "";
					using (PlayerBussiness db = new PlayerBussiness())
					{
						PlayerInfo tempSpouse = db.GetUserSingleByUserID(SpouseID);
						if (tempSpouse == null || tempSpouse.Sex == player.PlayerCharacter.Sex)
						{
							result2 = 1;
							return result2;
						}
						if (tempSpouse.IsMarried)
						{
							player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg2", new object[0]));
							result2 = 1;
							return result2;
						}
						ItemInfo WeddingRing = player.PropBag.GetItemByTemplateID(0, 11103);
						if (WeddingRing == null)
						{
							ShopItemInfo tempRing = ShopMgr.FindShopbyTemplatID(11103).FirstOrDefault<ShopItemInfo>();
							if (tempRing == null)
							{
								player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg6", new object[0]));
								result2 = 1;
								return result2;
							}
							if (player.PlayerCharacter.Money < tempRing.AValue1)
							{
								player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg1", new object[0]));
								result2 = 1;
								return result2;
							}
							removeRing = false;
						}
						MarryApplyInfo info = new MarryApplyInfo();
						info.UserID = SpouseID;
						info.ApplyUserID = player.PlayerCharacter.ID;
						info.ApplyUserName = player.PlayerCharacter.NickName;
						info.ApplyType = 1;
						info.LoveProclamation = loveProclamation;
						info.ApplyResult = false;
						int id = 0;
						if (db.SavePlayerMarryNotice(info, 0, ref id))
						{
							if (removeRing)
							{
								player.RemoveItem(WeddingRing, eItemRemoveType.Wedding);
							}
							else
							{
								ShopItemInfo tempRing = ShopMgr.FindShopbyTemplatID(11103).FirstOrDefault<ShopItemInfo>();
								player.OnUsingItem(tempRing.TemplateID);
								player.RemoveMoney(tempRing.AValue1, LogMoneyType.Marry, LogMoneyType.Marry_Spark);
							}
							player.Out.SendPlayerMarryApply(player, player.PlayerCharacter.ID, player.PlayerCharacter.NickName, loveProclamation, id);
							GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(SpouseID);
							SpouseName = tempSpouse.NickName;
							result = true;
							player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg3", new object[0]));
						}
					}
					if (result && Broadcast && SpouseName != "")
					{
						string msg = LanguageMgr.GetTranslation("MarryApplyHandler.Msg5", new object[]
						{
							player.PlayerCharacter.NickName,
							SpouseName
						});
						GSPacketIn pkg = new GSPacketIn(10);
						pkg.WriteInt(2);
						pkg.WriteString(msg);
						GameServer.Instance.LoginServer.SendPacket(pkg);
						GamePlayer[] players = WorldMgr.GetAllPlayers();
						GamePlayer[] array = players;
						for (int i = 0; i < array.Length; i++)
						{
							GamePlayer p = array[i];
							p.Out.SendTCP(pkg);
						}
					}
					result2 = 0;
				}
			}
			return result2;
		}
	}
}
