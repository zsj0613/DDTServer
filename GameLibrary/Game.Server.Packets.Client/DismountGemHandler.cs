using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(125, "取下宝石")]
	public class DismountGemHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int Place = packet.ReadInt();
			int Templateid = packet.ReadInt();
			ItemInfo Item = player.HideBag.GetItemAt(0);
			int result2;
			if (Place <= 0 || Templateid <= 0)
			{
				result2 = 0;
			}
			else
			{
				int NeedMoney = 500;
				if (player.PlayerCharacter.GiftToken < NeedMoney)
				{
					result2 = 0;
				}
				else
				{
					bool result = false;
					int Hole = this.GetItemHole(Place, Item);
					if (Hole == Templateid)
					{
						ItemTemplateInfo template = ItemMgr.FindItemTemplate(Templateid);
						if (template != null)
						{
							ItemInfo item = ItemInfo.CreateFromTemplate(template, 1, 119);
							if (item != null)
							{
								if (player.RemoveGiftToken(NeedMoney) == NeedMoney)
								{
									item.IsBinds = true;
									result = true;
									player.HideBag.UpdateItem(Item);
									if (!player.AddItem(item))
									{
										player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Game.Server.GameUtils.Title2", new object[0]));
										player.SendItemToMail(item, LanguageMgr.GetTranslation("DismountGemHandler.Full", new object[0]), item.Template.Name, eMailType.Default);
										player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
									}
									else
									{
										player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("DismountGem.success", new object[0]));
									}
								}
							}
						}
					}
					if (!result)
					{
						player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("DismountGem.Fail", new object[0]));
					}
					result2 = 0;
				}
			}
			return result2;
		}
		public int GetItemHole(int place, ItemInfo item)
		{
			int result;
			if (place > 0 && place < 7)
			{
				int template = 0;
				switch (place)
				{
				case 1:
					if (item.Hole1 > 0)
					{
						template = item.Hole1;
						item.Hole1 = 0;
					}
					result = template;
					return result;
				case 2:
					if (item.Hole2 > 0)
					{
						template = item.Hole2;
						item.Hole2 = 0;
					}
					result = template;
					return result;
				case 3:
					if (item.Hole3 > 0)
					{
						template = item.Hole3;
						item.Hole3 = 0;
					}
					result = template;
					return result;
				case 4:
					if (item.Hole4 > 0)
					{
						template = item.Hole4;
						item.Hole4 = 0;
					}
					result = template;
					return result;
				case 5:
					if (item.Hole5 > 0)
					{
						template = item.Hole5;
						item.Hole5 = 0;
					}
					result = template;
					return result;
				case 6:
					if (item.Hole6 > 0)
					{
						template = item.Hole6;
						item.Hole6 = 0;
					}
					result = template;
					return result;
				}
			}
			result = 0;
			return result;
		}
	}
}
