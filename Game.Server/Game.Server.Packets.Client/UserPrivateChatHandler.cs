using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(37, "用户与用户之间的聊天")]
	public class UserPrivateChatHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int id = packet.ReadInt();
			string nickName = packet.ReadString();
			string senderName = packet.ReadString();
			string msg = packet.ReadString();
			if (id == 0)
			{
				using (PlayerBussiness db = new PlayerBussiness())
				{
					PlayerInfo info = db.GetUserSingleByNickName(nickName);
					if (info != null)
					{
						id = info.ID;
					}
				}
			}
			int result;
			if (id != 0)
			{
				GSPacketIn pkg = packet.Clone();
				pkg.ClearContext();
				pkg.ClientID = player.PlayerCharacter.ID;
				pkg.WriteInt(id);
				pkg.WriteString(nickName);
				pkg.WriteString(player.PlayerCharacter.NickName);
				pkg.WriteString(msg);
				GamePlayer other = WorldMgr.GetPlayerById(id);
				if (other != null)
				{
					if (other.IsBlackFriend(player.PlayerCharacter.ID))
					{
						result = 1;
						return result;
					}
					other.Out.SendTCP(pkg);
				}
				else
				{
					GameServer.Instance.LoginServer.SendPacket(pkg);
				}
				player.Out.SendTCP(pkg);
			}
			else
			{
				player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserPrivateChatHandler.NoUser", new object[0]));
			}
			result = 1;
			return result;
		}
	}
}
