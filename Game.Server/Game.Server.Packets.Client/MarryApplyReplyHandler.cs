using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler(250, "求婚答复")]
	internal class MarryApplyReplyHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			bool result = packet.ReadBoolean();
			int UserID = packet.ReadInt();
			int AnswerId = packet.ReadInt();
			if (result && player.PlayerCharacter.SpouseID > 0)
			{
				player.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("MarryApplyReplyHandler.Msg2", new object[0]));
			}
			int result2;
			using (PlayerBussiness db = new PlayerBussiness())
			{
				PlayerInfo tempSpouse = db.GetUserSingleByUserID(UserID);
				if (!result)
				{
					this.SendGoodManCard(tempSpouse.NickName, tempSpouse.ID, player.PlayerCharacter.NickName, player.PlayerCharacter.ID, db);
					GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(tempSpouse.ID);
				}
				if (tempSpouse == null || tempSpouse.Sex == player.PlayerCharacter.Sex)
				{
					result2 = 1;
					return result2;
				}
				if (tempSpouse.SpouseID > 0)
				{
					player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryApplyReplyHandler.Msg3", new object[0]));
				}
				MarryApplyInfo info = new MarryApplyInfo();
				info.UserID = UserID;
				info.ApplyUserID = player.PlayerCharacter.ID;
				info.ApplyUserName = player.PlayerCharacter.NickName;
				info.ApplyType = 2;
				info.LoveProclamation = "";
				info.ApplyResult = result;
				int id = 0;
				if (db.SavePlayerMarryNotice(info, AnswerId, ref id))
				{
					if (result)
					{
						player.Out.SendMarryApplyReply(player, tempSpouse.ID, tempSpouse.NickName, result, false, id);
						player.LoadMarryProp();
						this.SendSYSMessages(player, tempSpouse);
					}
					GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(tempSpouse.ID);
					result2 = 0;
					return result2;
				}
			}
			result2 = 1;
			return result2;
		}
		public void SendSYSMessages(GamePlayer player, PlayerInfo spouse)
		{
			string name = player.PlayerCharacter.Sex ? player.PlayerCharacter.NickName : spouse.NickName;
			string name2 = player.PlayerCharacter.Sex ? spouse.NickName : player.PlayerCharacter.NickName;
			string msg = LanguageMgr.GetTranslation("MarryApplyReplyHandler.Msg1", new object[]
			{
				name,
				name2
			});
			player.OnPlayerMarry();
			GSPacketIn pkg = new GSPacketIn(10);
			pkg.WriteInt(2);
			pkg.WriteString(msg);
			GameServer.Instance.LoginServer.SendPacket(pkg);
			GamePlayer[] players = WorldMgr.GetAllPlayers();
			GamePlayer[] array = players;
			for (int i = 0; i < array.Length; i++)
			{
				GamePlayer p = array[i];
				p.Out.SendTCP(pkg);
			}
		}
		public void SendGoodManCard(string receiverName, int receiverID, string senderName, int senderID, PlayerBussiness db)
		{
			ItemTemplateInfo goodMan = ItemMgr.FindItemTemplate(11105);
			ItemInfo goodManCard = ItemInfo.CreateFromTemplate(goodMan, 1, 112);
			goodManCard.IsBinds = true;
			GamePlayer player = WorldMgr.GetPlayerById(receiverID);
			goodManCard.UserID = 0;
			db.AddGoods(goodManCard);
			MailInfo mail = new MailInfo();
			mail.Annex1 = goodManCard.ItemID.ToString();
			mail.Content = LanguageMgr.GetTranslation("MarryApplyReplyHandler.Content", new object[0]);
			mail.Gold = 0;
			mail.IsExist = true;
			mail.Money = 0;
			mail.GiftToken = 0;
			mail.Receiver = receiverName;
			mail.ReceiverID = receiverID;
			mail.Sender = senderName;
			mail.SenderID = senderID;
			mail.Title = LanguageMgr.GetTranslation("MarryApplyReplyHandler.Title", new object[0]);
			mail.Type = 14;
			if (db.SendMail(mail) && player != null)
			{
				player.Out.SendMailResponse(mail.ReceiverID, eMailRespose.Receiver);
			}
		}
	}
}
