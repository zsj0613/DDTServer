using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(188, "User ConsotiaName Change")]
	public class UserChangeConsotiaNameHandler : IPacketHandler
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
				int consortiaID = packet.ReadInt();
				ConsortiaInfo consortia = ConsortiaMgr.FindConsortiaInfo(consortiaID);
				if (consortia.ChairmanName != client.Player.PlayerCharacter.NickName)
				{
					result = 0;
				}
				else
				{
					eBageType bagType = (eBageType)packet.ReadByte();
					int place = packet.ReadInt();
					string newConsotiaName = packet.ReadString();
					string message = "";
					ItemInfo info = client.Player.GetItemAt(bagType, place);
					if (info != null)
					{
						if (info.TemplateID == 11993)
						{
							string userName = client.Player.PlayerCharacter.UserName;
							string nickName = client.Player.PlayerCharacter.NickName;
							using (PlayerBussiness db = new PlayerBussiness())
							{
								if (db.RenameConsortiaNameByCard(userName, nickName, newConsotiaName, ref message))
								{
									client.Player.RemoveItem(info, eItemRemoveType.Use);
									packet.WriteString(LanguageMgr.GetTranslation("Tank.Request.RenameConsortiaName.Success", new object[0]));
									client.Out.SendTCP(packet);
								}
								else
								{
									client.Out.SendMessage(eMessageType.Normal, message);
								}
							}
						}
					}
					result = 0;
				}
			}
			return result;
		}
	}
}
