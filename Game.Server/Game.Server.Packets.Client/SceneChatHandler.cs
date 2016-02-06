using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Managers;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(19, "用户场景聊天")]
	public class SceneChatHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			packet.ClientID = player.PlayerCharacter.ID;
			byte channel = packet.ReadByte();
			bool team = packet.ReadBoolean();
			string nick = packet.ReadString();
			string msg = packet.ReadString();
			GSPacketIn pkg = packet.Clone();
			pkg.ClearContext();
			pkg.ClientID = player.PlayerCharacter.ID;
			pkg.WriteInt(player.AreaID);
			pkg.WriteByte(channel);
			pkg.WriteBoolean(team);
			pkg.WriteString(player.PlayerCharacter.NickName);
			pkg.WriteString(msg);
			int result;
			if (player.CurrentRoom != null && channel != 3 && channel != 9)
			{
				if (player.CurrentRoom.RoomType == eRoomType.Match)
				{
					if (player.CurrentRoom.Game != null)
					{
						player.CurrentRoom.BattleServer.Server.SendChatMessage(msg, player, team, channel);
						result = 1;
						return result;
					}
				}
			}
			if (channel == 3)
			{
				if (player.PlayerCharacter.ConsortiaID == 0)
				{
					result = 0;
					return result;
				}
				if (player.PlayerCharacter.IsBanChat)
				{
					player.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ConsortiaChatHandler.IsBanChat", new object[0]));
					result = 1;
					return result;
				}
				pkg.WriteInt(player.PlayerCharacter.ConsortiaID);
				GamePlayer[] players = WorldMgr.GetAllPlayers();
				GamePlayer[] array = players;
				for (int i = 0; i < array.Length; i++)
				{
					GamePlayer p = array[i];
					if (p.PlayerCharacter.ConsortiaID == player.PlayerCharacter.ConsortiaID && !p.IsBlackFriend(player.PlayerCharacter.ID))
					{
						p.Out.SendTCP(pkg);
					}
				}
				GameServer.Instance.LoginServer.SendPacket(pkg);
			}
			else
			{
				if (channel == 9)
				{
					if (player.CurrentMarryRoom == null)
					{
						result = 1;
						return result;
					}
					player.CurrentMarryRoom.SendToAllForScene(pkg, player.MarryMap);
				}
				else
				{
					if (channel == 13)
					{
						if (player.CurrentSpaRoom == null)
						{
							result = 1;
							return result;
						}
						player.CurrentSpaRoom.SendToRoomPlayer(pkg);
					}
					else
					{
						if (player.CurrentRoom != null)
						{
							if (team)
							{
								player.CurrentRoom.SendToTeam(pkg, player.CurrentRoomTeam, player);
							}
							else
							{
								player.CurrentRoom.SendToAll(pkg);
							}
						}
						else
						{
							if (DateTime.Compare(player.LastChatTime.AddSeconds(1.0), DateTime.Now) > 0)
							{
								result = 1;
								return result;
							}
							if (team)
							{
								result = 1;
								return result;
							}
							if (DateTime.Compare(player.LastChatTime.AddSeconds(30.0), DateTime.Now) > 0)
							{
								player.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("SceneChatHandler.Fast", new object[0]));
								result = 1;
								return result;
							}
							player.LastChatTime = DateTime.Now;
							GamePlayer[] list = WorldMgr.GetAllPlayers();
							GamePlayer[] array = list;
							for (int i = 0; i < array.Length; i++)
							{
								GamePlayer p = array[i];
								if (p.CurrentRoom == null && p.CurrentMarryRoom == null && p.CurrentSpaRoom == null && !p.IsBlackFriend(player.PlayerCharacter.ID))
								{
									p.Out.SendTCP(pkg);
								}
							}
							player.PlayerCharacter.ChatCount++;
							player.Out.SendCheckCode();
						}
					}
				}
			}
			result = 1;
			return result;
		}
	}
}
