using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(160, "添加好友")]
	public class FriendAddHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			GSPacketIn pkg = packet.Clone();
			pkg.ClearContext();
			string nickName = packet.ReadString();
			int relation = packet.ReadInt();
			int result;
			if (relation < 0 || relation > 1)
			{
				result = 1;
			}
			else
			{
				using (PlayerBussiness db = new PlayerBussiness())
				{
					GamePlayer other = WorldMgr.GetClientByPlayerNickName(nickName);
					PlayerInfo user;
					if (other != null)
					{
						user = other.PlayerCharacter;
					}
					else
					{
						user = db.GetUserSingleByNickName(nickName);
					}
					if (!string.IsNullOrEmpty(nickName) && user != null)
					{
						if (!player.Friends.ContainsKey(user.ID) || player.Friends[user.ID] != relation)
						{
							if (db.AddFriends(new FriendInfo
							{
								FriendID = user.ID,
								IsExist = true,
								Remark = "",
								UserID = player.PlayerCharacter.ID,
								Relation = relation
							}))
							{
								player.FriendsAdd(user.ID, relation);
								pkg.WriteInt(user.ID);
								pkg.WriteString(user.NickName);
								pkg.WriteBoolean(user.Sex);
								pkg.WriteString(user.Style);
								pkg.WriteString(user.Colors);
								pkg.WriteString(user.Skin);
								pkg.WriteInt((user.State == 1) ? 1 : 0);
								pkg.WriteInt(user.Grade);
								pkg.WriteInt(user.Hide);
								pkg.WriteString(user.ConsortiaName);
								pkg.WriteInt(user.Total);
								pkg.WriteInt(user.Escape);
								pkg.WriteInt(user.Win);
								pkg.WriteInt(user.Offer);
								pkg.WriteInt(user.Repute);
								pkg.WriteInt(relation);
								pkg.WriteString(user.UserName);
								pkg.WriteInt(user.Nimbus);
								pkg.WriteInt(user.FightPower);
                                pkg.WriteInt(user.AchievementPoint);
                                pkg.WriteString(user.Honor);
								if (relation != 1 && user.State != 0)
								{
									GSPacketIn response = new GSPacketIn(166, player.PlayerCharacter.ID);
									response.WriteInt(user.ID);
									response.WriteString(player.PlayerCharacter.NickName);
									if (other != null)
									{
										other.Out.SendTCP(response);
									}
									else
									{
										GameServer.Instance.LoginServer.SendPacket(response);
									}
								}
								player.Out.SendTCP(pkg);
							}
						}
						else
						{
							player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("FriendAddHandler.Falied", new object[0]));
						}
					}
					else
					{
						player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("FriendAddHandler.Success", new object[0]));
					}
				}
				result = 0;
			}
			return result;
		}
	}
}
