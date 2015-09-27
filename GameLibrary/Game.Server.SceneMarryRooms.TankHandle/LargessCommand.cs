using Bussiness;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(5)]
	public class LargessCommand : IMarryCommandHandler
	{
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			bool result;
			if (player.CurrentMarryRoom == null)
			{
				result = false;
			}
			else
			{
				int num = packet.ReadInt();
				if (num > 0)
				{
					if (player.PlayerCharacter.Money >= num)
					{
						player.RemoveMoney(num, LogMoneyType.Marry, LogMoneyType.Marry_Gift);
						using (PlayerBussiness pb = new PlayerBussiness())
						{
							string content = LanguageMgr.GetTranslation("LargessCommand.Content", new object[]
							{
								player.PlayerCharacter.NickName,
								num / 2
							});
							string title = LanguageMgr.GetTranslation("LargessCommand.Title", new object[]
							{
								player.PlayerCharacter.NickName
							});
							MailInfo mail = new MailInfo();
							mail.Annex1 = "";
							mail.Content = content;
							mail.Gold = 0;
							mail.IsExist = true;
							mail.Money = num / 2;
							mail.GiftToken = 0;
							mail.Receiver = player.CurrentMarryRoom.Info.BrideName;
							mail.ReceiverID = player.CurrentMarryRoom.Info.BrideID;
							mail.Sender = LanguageMgr.GetTranslation("LargessCommand.Sender", new object[0]);
							mail.SenderID = 0;
							mail.Title = title;
							mail.Type = 14;
							pb.SendMail(mail);
							player.Out.SendMailResponse(mail.ReceiverID, eMailRespose.Receiver);
							MailInfo mail2 = new MailInfo();
							mail2.Annex1 = "";
							mail2.Content = content;
							mail2.Gold = 0;
							mail2.IsExist = true;
							mail2.Money = num / 2;
							mail2.GiftToken = 0;
							mail2.Receiver = player.CurrentMarryRoom.Info.GroomName;
							mail2.ReceiverID = player.CurrentMarryRoom.Info.GroomID;
							mail2.Sender = LanguageMgr.GetTranslation("LargessCommand.Sender", new object[0]);
							mail2.SenderID = 0;
							mail2.Title = title;
							mail2.Type = 14;
							pb.SendMail(mail2);
							player.Out.SendMailResponse(mail2.ReceiverID, eMailRespose.Receiver);
						}
						player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("LargessCommand.Succeed", new object[0]));
						GSPacketIn msg = player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("LargessCommand.Notice", new object[]
						{
							player.PlayerCharacter.NickName,
							num
						}));
						player.CurrentMarryRoom.SendToPlayerExceptSelf(msg, player);
						result = true;
					}
					else
					{
						player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserFirecrackersCommand.MoneyNotEnough", new object[0]));
						result = false;
					}
				}
				else
				{
					result = false;
				}
			}
			return result;
		}
	}
}
