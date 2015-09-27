using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using System;
using Game.Language;
namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(2)]
	public class HymenealCommand : IMarryCommandHandler
	{
		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			bool result2;
			if (player.CurrentMarryRoom == null || player.CurrentMarryRoom.RoomState != eRoomState.FREE)
			{
				result2 = false;
			}
			else
			{
				if (player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID && player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID)
				{
					result2 = false;
				}
				else
				{
					int needMoney = GameProperties.PRICE_PROPOSE;
					if (player.CurrentMarryRoom.Info.IsHymeneal)
					{
						if (player.PlayerCharacter.Money < needMoney)
						{
							player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("UserFirecrackersCommand.MoneyNotEnough", new object[0]));
							result2 = false;
							return result2;
						}
					}
					GamePlayer Groom = player.CurrentMarryRoom.GetPlayerByUserID(player.CurrentMarryRoom.Info.GroomID);
					if (Groom == null)
					{
						player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("HymenealCommand.NoGroom", new object[0]));
						result2 = false;
					}
					else
					{
						GamePlayer Bride = player.CurrentMarryRoom.GetPlayerByUserID(player.CurrentMarryRoom.Info.BrideID);
						if (Bride == null)
						{
							player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("HymenealCommand.NoBride", new object[0]));
							result2 = false;
						}
						else
						{
							bool result = false;
							bool isFirst = false;
							GSPacketIn pkg = packet.Clone();
							int hymenealState = packet.ReadInt();
							if (1 == hymenealState)
							{
								player.CurrentMarryRoom.RoomState = eRoomState.FREE;
							}
							else
							{
								player.CurrentMarryRoom.RoomState = eRoomState.Hymeneal;
								player.CurrentMarryRoom.BeginTimerForHymeneal(170000);
								if (!player.PlayerCharacter.IsGotRing)
								{
									isFirst = true;
									ItemTemplateInfo ringTemplate = ItemMgr.FindItemTemplate(9022);
									ItemInfo ring = ItemInfo.CreateFromTemplate(ringTemplate, 1, 112);
									ring.IsBinds = true;
									using (PlayerBussiness pb = new PlayerBussiness())
									{
										ring.UserID = 0;
										pb.AddGoods(ring);
										string content = LanguageMgr.GetTranslation("HymenealCommand.Content", new object[]
										{
											Bride.PlayerCharacter.NickName
										});
										MailInfo mail = new MailInfo();
										mail.Annex1 = ring.ItemID.ToString();
										mail.Content = content;
										mail.Gold = 0;
										mail.IsExist = true;
										mail.Money = 0;
										mail.GiftToken = 0;
										mail.Receiver = Groom.PlayerCharacter.NickName;
										mail.ReceiverID = Groom.PlayerCharacter.ID;
										mail.Sender = LanguageMgr.GetTranslation("HymenealCommand.Sender", new object[0]);
										mail.SenderID = 0;
										mail.Title = LanguageMgr.GetTranslation("HymenealCommand.Title", new object[0]);
										mail.Type = 14;
										if (pb.SendMail(mail))
										{
											result = true;
										}
										player.Out.SendMailResponse(mail.ReceiverID, eMailRespose.Receiver);
									}
									ItemInfo ring2 = ItemInfo.CreateFromTemplate(ringTemplate, 1, 112);
									ring2.IsBinds = true;
									using (PlayerBussiness pb = new PlayerBussiness())
									{
										ring2.UserID = 0;
										pb.AddGoods(ring2);
										string content = LanguageMgr.GetTranslation("HymenealCommand.Content", new object[]
										{
											Groom.PlayerCharacter.NickName
										});
										MailInfo mail = new MailInfo();
										mail.Annex1 = ring2.ItemID.ToString();
										mail.Content = content;
										mail.Gold = 0;
										mail.IsExist = true;
										mail.Money = 0;
										mail.GiftToken = 0;
										mail.Receiver = Bride.PlayerCharacter.NickName;
										mail.ReceiverID = Bride.PlayerCharacter.ID;
										mail.Sender = LanguageMgr.GetTranslation("HymenealCommand.Sender", new object[0]);
										mail.SenderID = 0;
										mail.Title = LanguageMgr.GetTranslation("HymenealCommand.Title", new object[0]);
										mail.Type = 14;
										if (pb.SendMail(mail))
										{
											result = true;
										}
										player.Out.SendMailResponse(mail.ReceiverID, eMailRespose.Receiver);
									}
									player.CurrentMarryRoom.Info.IsHymeneal = true;
									using (PlayerBussiness db = new PlayerBussiness())
									{
										db.UpdateMarryRoomInfo(player.CurrentMarryRoom.Info);
										db.UpdatePlayerGotRingProp(Groom.PlayerCharacter.ID, Bride.PlayerCharacter.ID);
										Groom.LoadMarryProp();
										Bride.LoadMarryProp();
									}
								}
								else
								{
									isFirst = false;
									result = true;
								}
								if (!isFirst)
								{
									player.RemoveMoney(needMoney, LogMoneyType.Marry, LogMoneyType.Marry_Hymeneal);
									CountBussiness.InsertSystemPayCount(player.PlayerCharacter.ID, needMoney, 0, 0, 1);
								}
								pkg.WriteInt(player.CurrentMarryRoom.Info.ID);
								pkg.WriteBoolean(result);
								player.CurrentMarryRoom.SendToAll(pkg);
								if (result)
								{
									string msg = LanguageMgr.GetTranslation("HymenealCommand.Succeed", new object[]
									{
										Groom.PlayerCharacter.NickName,
										Bride.PlayerCharacter.NickName
									});
									GSPacketIn message = player.Out.SendMessage(eMessageType.ChatNormal, msg);
									player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(message, player);
								}
							}
							result2 = true;
						}
					}
				}
			}
			return result2;
		}
	}
}
