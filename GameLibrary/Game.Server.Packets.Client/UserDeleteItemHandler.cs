using Bussiness;
using Game.Base.Packets;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(42, "删除物品")]
	public class UserDeleteItemHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int result;
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
				result = 0;
			}
			else
			{
				int bagType = (int)packet.ReadByte();
				int place = packet.ReadInt();
				result = 0;
			}
			return result;
		}
	}
}
