using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(171, "User NickName Change")]
	public class UserChangeNickNameHandler : IPacketHandler
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
				eBageType bagType = (eBageType)packet.ReadByte();
				int place = packet.ReadInt();
				string newNickName = packet.ReadString();
				string message = "";
				ItemInfo info = client.Player.GetItemAt(bagType, place);
				if (info != null)
				{
					if (info.TemplateID == 11994)
					{
						string oldNickName = client.Player.PlayerCharacter.NickName;
						string userName = client.Player.PlayerCharacter.UserName;
						using (PlayerBussiness db = new PlayerBussiness())
						{
							if (db.RenameByCard(userName, oldNickName, newNickName, ref message))
							{
								client.Player.OnUsingItem(info.TemplateID);
								client.Player.RemoveItem(info, eItemRemoveType.Use);
								packet.WriteString(LanguageMgr.GetTranslation("Tank.Request.RenameNick.Success", new object[0]));
								client.Out.SendTCP(packet);
							}
						}
					}
				}
				result = 0;
			}
			return result;
		}
	}
}
