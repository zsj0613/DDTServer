using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(155, "公会聊天")]
	public class ConsortiaChatHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result;
			if (player.PlayerCharacter.ConsortiaID == 0)
			{
				result = 0;
			}
			else
			{
				if (player.PlayerCharacter.IsBanChat)
				{
					player.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ConsortiaChatHandler.IsBanChat", new object[0]));
					result = 1;
				}
				else
				{
					packet.ClientID = player.PlayerCharacter.ID;
					byte channel = packet.ReadByte();
					string nick = packet.ReadString();
					string msg = packet.ReadString();
					packet.WriteInt(player.PlayerCharacter.ConsortiaID);
					GamePlayer[] players = WorldMgr.GetAllPlayers();
					GamePlayer[] array = players;
					for (int i = 0; i < array.Length; i++)
					{
						GamePlayer p = array[i];
						if (p.PlayerCharacter.ConsortiaID == player.PlayerCharacter.ConsortiaID)
						{
							p.Out.SendTCP(packet);
						}
					}
					GameServer.Instance.LoginServer.SendPacket(packet);
					result = 0;
				}
			}
			return result;
		}
	}
}
