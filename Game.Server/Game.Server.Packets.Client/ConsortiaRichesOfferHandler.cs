using Bussiness;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(135, "捐献公会财富")]
	public class ConsortiaRichesOfferHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result2;
			if (player.PlayerCharacter.ConsortiaID == 0)
			{
				result2 = 0;
			}
			else
			{
				int money = packet.ReadInt();
				if (player.PlayerCharacter.HasBagPassword && player.PlayerCharacter.IsLocked)
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
					result2 = 1;
				}
				else
				{
					if (money < 1 || player.PlayerCharacter.Money < money)
					{
						player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaRichesOfferHandler.NoMoney", new object[0]));
						result2 = 1;
					}
					else
					{
						bool result = false;
						string msg = "ConsortiaRichesOfferHandler.Failed";
						using (ConsortiaBussiness db = new ConsortiaBussiness())
						{
							int riches = money * 100;
							if (db.ConsortiaRichAdd(player.PlayerCharacter.ConsortiaID, ref riches, 5, player.PlayerCharacter.NickName))
							{
								result = true;
								player.PlayerCharacter.RichesOffer += riches;
								player.RemoveMoney(money, LogMoneyType.Consortia, LogMoneyType.Consortia_Rich);
								player.OnPlayerAddItem("RichesOffer", riches);
								msg = "ConsortiaRichesOfferHandler.Successed";
								GameServer.Instance.LoginServer.SendConsortiaRichesOffer(player.PlayerCharacter.ConsortiaID, player.PlayerCharacter.ID, player.PlayerCharacter.NickName, riches);
							}
						}
						packet.WriteBoolean(result);
						packet.WriteString(LanguageMgr.GetTranslation(msg, new object[0]));
						player.Out.SendTCP(packet);
						result2 = 0;
					}
				}
			}
			return result2;
		}
	}
}
