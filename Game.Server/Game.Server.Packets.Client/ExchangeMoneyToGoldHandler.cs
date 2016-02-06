using Bussiness;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(126, "点券兑换金币")]
	public class ExchangeMoneyToGoldHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int count = packet.ReadInt();
			int money = 200;
			int result;
			if (count < 0)
			{
				result = 0;
			}
			else
			{
				money *= count;
				int gold = 10000 * count;
				if (player.PlayerCharacter.Money >= money)
				{
					if (player.RemoveMoney(money, LogMoneyType.Shop, LogMoneyType.MoneyToGold) == money)
					{
						player.AddGold(gold);
						player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ExchangeMoneyToGoldHandler.success", new object[]
						{
							gold
						}));
					}
				}
				else
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ExchangeMoneyToGoldHandler.nomoney", new object[0]));
				}
				result = 0;
			}
			return result;
		}
	}
}
