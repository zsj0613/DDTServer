using Bussiness;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Language;
namespace Game.Server.Packets.Client
{
	[PacketHandler(113, "获取邮件到背包")]
	public class MailGetAttachHandler : AbstractPlayerPacketHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result2;
			if (player.PlayerState != ePlayerState.Playing)
			{
				result2 = 0;
			}
			else
			{
				if (player.PlayerCharacter.HasBagPassword && player.PlayerCharacter.IsLocked)
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
					result2 = 0;
				}
				else
				{
					int mailId = packet.ReadInt();
					byte attachIndex = packet.ReadByte();
					GSPacketIn pkg = packet.Clone();
					pkg.ClearContext();
					using (PlayerBussiness db = new PlayerBussiness())
					{
						MailInfo mes = db.GetMailSingle(player.PlayerCharacter.ID, mailId);
						if (mes != null && mes.ReceiverID == player.PlayerId)
						{
							List<int> getIndexes = new List<int>();
							string msg = "";
							eMessageType eMsg = eMessageType.Normal;
							bool result = true;
							int oldMoney = mes.Money;
							if (mes.Type > 100 && mes.Money > player.PlayerCharacter.Money)
							{
								player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MailGetAttachHandler.NoMoney", new object[0]));
								result2 = 0;
								return result2;
							}
							if (!mes.IsRead)
							{
								mes.IsRead = true;
								mes.ValidDate = 72;
								mes.SendTime = DateTime.Now;
							}
							if (result && (attachIndex == 0 || attachIndex == 1) && !string.IsNullOrEmpty(mes.Annex1))
							{
								result = this.GetAnnex(db, mes.Annex1, player, ref msg, ref eMsg);
								if (result)
								{
									getIndexes.Add(1);
									mes.Annex1 = null;
								}
							}
							if (result && (attachIndex == 0 || attachIndex == 2) && !string.IsNullOrEmpty(mes.Annex2))
							{
								result = this.GetAnnex(db, mes.Annex2, player, ref msg, ref eMsg);
								if (result)
								{
									getIndexes.Add(2);
									mes.Annex2 = null;
								}
							}
							if (result && (attachIndex == 0 || attachIndex == 3) && !string.IsNullOrEmpty(mes.Annex3))
							{
								result = this.GetAnnex(db, mes.Annex3, player, ref msg, ref eMsg);
								if (result)
								{
									getIndexes.Add(3);
									mes.Annex3 = null;
								}
							}
							if (result && (attachIndex == 0 || attachIndex == 4) && !string.IsNullOrEmpty(mes.Annex4))
							{
								result = this.GetAnnex(db, mes.Annex4, player, ref msg, ref eMsg);
								if (result)
								{
									getIndexes.Add(4);
									mes.Annex4 = null;
								}
							}
							if (result && (attachIndex == 0 || attachIndex == 5) && !string.IsNullOrEmpty(mes.Annex5))
							{
								result = this.GetAnnex(db, mes.Annex5, player, ref msg, ref eMsg);
								if (result)
								{
									getIndexes.Add(5);
									mes.Annex5 = null;
								}
							}
							if ((attachIndex == 0 || attachIndex == 6) && mes.Gold > 0)
							{
								getIndexes.Add(6);
								player.AddGold(mes.Gold);
								mes.Gold = 0;
							}
							if ((attachIndex == 0 || attachIndex == 7) && mes.Type < 100 && mes.Money > 0)
							{
								int money = player.PlayerCharacter.Money;
								getIndexes.Add(7);
								player.AddMoney(mes.Money, LogMoneyType.Mail, LogMoneyType.Mail_Money);
								if (player.PlayerCharacter.Money - money == mes.Money)
								{
									player.SaveIntoDatabase();
									mes.Money = 0;
								}
							}
							if ((attachIndex == 0 || attachIndex == 8) && mes.GiftToken > 0)
							{
								getIndexes.Add(8);
								player.AddGiftToken(mes.GiftToken);
								mes.GiftToken = 0;
							}
							if (mes.Type > 100 && mes.Money > 0)
							{
								mes.Money = 0;
								msg = LanguageMgr.GetTranslation("MailGetAttachHandler.Deduct", new object[0]) + (string.IsNullOrEmpty(msg) ? LanguageMgr.GetTranslation("MailGetAttachHandler.Success", new object[0]) : msg);
							}
							if (db.UpdateMail(mes, oldMoney))
							{
								if (mes.Type > 100 && oldMoney > 0)
								{
									player.RemoveMoney(oldMoney, LogMoneyType.Mail, LogMoneyType.Mail_Pay);
									player.Out.SendMailResponse(mes.SenderID, eMailRespose.Receiver);
									player.Out.SendMailResponse(mes.ReceiverID, eMailRespose.Send);
								}
							}
							pkg.WriteInt(mailId);
							pkg.WriteInt(getIndexes.Count);
							foreach (int i in getIndexes)
							{
								pkg.WriteInt(i);
							}
							player.Out.SendTCP(pkg);
							player.Out.SendMessage(eMsg, string.IsNullOrEmpty(msg) ? LanguageMgr.GetTranslation("MailGetAttachHandler.Success", new object[0]) : msg);
						}
						else
						{
							player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("MailGetAttachHandler.error", new object[0]));
							MailGetAttachHandler.log.ErrorFormat("User want to get attachment failed: userid :{0}  mailId:{1} attachIndex:{2}", player.PlayerId, mailId, attachIndex);
						}
					}
					result2 = 0;
				}
			}
			return result2;
		}
		public bool GetAnnex(PlayerBussiness db, string value, GamePlayer player, ref string msg, ref eMessageType eMsg)
		{
			int gid = int.Parse(value);
			ItemInfo goods = db.GetUserItemSingle(gid);
			bool result;
			if (goods != null)
			{
				if (player.AddItem(goods))
				{
					eMsg = eMessageType.Normal;
					result = true;
					return result;
				}
				eMsg = eMessageType.ERROR;
				msg = LanguageMgr.GetTranslation(goods.GetBagName(), new object[0]) + LanguageMgr.GetTranslation("MailGetAttachHandler.NoPlace", new object[0]);
			}
			result = false;
			return result;
		}
	}
}
