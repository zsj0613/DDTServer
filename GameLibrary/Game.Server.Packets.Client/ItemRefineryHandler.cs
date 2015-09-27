using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(110, "物品炼化")]
	public class ItemRefineryHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result;
			if (player.PlayerCharacter.HasBagPassword && player.PlayerCharacter.IsLocked)
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.locked", new object[0]));
				result = 1;
			}
			else
			{
				result = 0;
			}
			return result;
		}
	}
}
