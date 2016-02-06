using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
	[PacketHandler(119, "物品比较")]
	public class ItemCompareHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int type = packet.ReadInt();
			int result;
			if (type == 2)
			{
				int itemID = packet.ReadInt();
				using (PlayerBussiness db = new PlayerBussiness())
				{
					ItemInfo item = db.GetUserItemSingle(itemID);
					if (item != null)
					{
						GSPacketIn pkg = new GSPacketIn(119, player.PlayerCharacter.ID);
						pkg.WriteInt(item.TemplateID);
						pkg.WriteInt(item.ItemID);
						pkg.WriteInt(item.StrengthenLevel);
						pkg.WriteInt(item.AttackCompose);
						pkg.WriteInt(item.AgilityCompose);
						pkg.WriteInt(item.LuckCompose);
						pkg.WriteInt(item.DefendCompose);
						pkg.WriteInt(item.ValidDate);
						pkg.WriteBoolean(item.IsBinds);
						pkg.WriteBoolean(item.IsJudge);
						pkg.WriteBoolean(item.IsUsed);
						if (item.IsUsed)
						{
							pkg.WriteString(item.BeginDate.ToString());
						}
						pkg.WriteInt(item.Hole1);
						pkg.WriteInt(item.Hole2);
						pkg.WriteInt(item.Hole3);
						pkg.WriteInt(item.Hole4);
						pkg.WriteInt(item.Hole5);
						pkg.WriteInt(item.Hole6);
						pkg.WriteString(item.Template.Hole);
						player.Out.SendTCP(pkg);
					}
					result = 1;
					return result;
				}
			}
			result = 0;
			return result;
		}
	}
}
