using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using System;
using System.Linq;

namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(6)]
	public class UserFirecrackersCommand : IMarryCommandHandler
	{
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			bool result;
			if (player.CurrentMarryRoom != null)
			{
				int userID = packet.ReadInt();
				int templateID = packet.ReadInt();
				ShopItemInfo temp = ShopMgr.FindShopbyTemplatID(templateID).FirstOrDefault<ShopItemInfo>();
				if (temp != null)
				{
					if (temp.APrice1 == -2)
					{
						if (player.PlayerCharacter.Gold >= temp.AValue1)
						{
							player.RemoveGold(temp.AValue1);
							player.CurrentMarryRoom.ReturnPacketForScene(player, packet);
							player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("UserFirecrackersCommand.Successed1", new object[]
							{
								temp.AValue1
							}));
							player.OnUsingItem(temp.TemplateID);
							result = true;
							return result;
						}
						player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserFirecrackersCommand.GoldNotEnough", new object[0]));
					}
					if (temp.APrice1 == -1)
					{
						if (player.PlayerCharacter.Money >= temp.AValue1)
						{
							player.RemoveMoney(temp.AValue1, LogMoneyType.Marry, LogMoneyType.Marry_Flower);
							player.CurrentMarryRoom.ReturnPacketForScene(player, packet);
							player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("UserFirecrackersCommand.Successed2", new object[]
							{
								temp.AValue1
							}));
							player.OnUsingItem(temp.TemplateID);
							result = true;
							return result;
						}
						player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserFirecrackersCommand.MoneyNotEnough", new object[0]));
					}
				}
			}
			result = false;
			return result;
		}
	}
}
