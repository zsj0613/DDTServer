using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(71, "小喇叭")]
	public class SmallBugleHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			ItemInfo item = player.PropBag.GetItemByCategoryID(0, 11, 4);
			if (item != null)
			{
				player.PropBag.RemoveCountFromStack(item, 1, eItemRemoveType.Use);
				player.OnUsingItem(item.TemplateID);
				int senderID = packet.ReadInt();
				string senderName = packet.ReadString();
				string msg = packet.ReadString();
				GSPacketIn pkg = packet.Clone();
				pkg.ClearContext();
				pkg.ClientID = player.PlayerCharacter.ID;
				pkg.WriteInt(player.PlayerCharacter.ID);
				pkg.WriteString(player.PlayerCharacter.NickName);
				pkg.WriteString(msg);
				GamePlayer[] players = WorldMgr.GetAllPlayers();
				GamePlayer[] array = players;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer p = array[i];
					p.Out.SendTCP(pkg);
				}
			}
			return 0;
		}
	}
}
