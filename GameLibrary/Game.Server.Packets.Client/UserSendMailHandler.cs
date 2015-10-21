using Bussiness;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Server.Packets.Client
{
	[PacketHandler(116, "发送邮件")]
	public class UserSendMailHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int result;
			if (player.PlayerCharacter.Gold < 100)
			{
				result = 1;
			}
			else
			{
				string msg = "UserSendMailHandler.Success";
				eMessageType eMsg = eMessageType.Normal;
				GSPacketIn pkg = packet.Clone();
				pkg.ClearContext();
				string nickName = packet.ReadString().Trim();
				string title = packet.ReadString();
				string content = packet.ReadString();
				bool isPay = packet.ReadBoolean();
				int validDate = packet.ReadInt();
				int money = packet.ReadInt();
				eBageType bag = (eBageType)packet.ReadByte();
				int place = packet.ReadInt();
				eBageType bag2 = (eBageType)packet.ReadByte();
				int place2 = packet.ReadInt();
				eBageType bag3 = (eBageType)packet.ReadByte();
				int place3 = packet.ReadInt();
				eBageType bag4 = (eBageType)packet.ReadByte();
				int place4 = packet.ReadInt();
				if ((money != 0 || place != -1 || place2 != -1 || place3 != -1 || place4 != -1) && player.PlayerCharacter.HasBagPassword && player.PlayerCharacter.IsLocked)
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked", new object[0]));
					pkg.WriteBoolean(false);
					player.Out.SendTCP(pkg);
					result = 1;
				}
				else
				{
					using (PlayerBussiness db = new PlayerBussiness())
					{
						GamePlayer otherp = WorldMgr.GetClientByPlayerNickName(nickName);
						PlayerInfo user;
						if (otherp == null)
						{
							user = db.GetUserSingleByNickName(nickName);
						}
						else
						{
							user = otherp.PlayerCharacter;
						}
						if (!string.IsNullOrEmpty(title))
						{
							if (user != null && !string.IsNullOrEmpty(nickName))
							{
								if (user.NickName != player.PlayerCharacter.NickName)
								{
									player.SaveIntoDatabase();
									MailInfo message = new MailInfo();
									message.SenderID = player.PlayerCharacter.ID;
									message.Sender = player.PlayerCharacter.NickName;
									message.ReceiverID = user.ID;
									message.Receiver = user.NickName;
									message.IsExist = true;
									message.Gold = 0;
									message.Money = 0;
									message.Title = title;
									message.Content = content;
									List<ItemInfo> items = new List<ItemInfo>();
									List<eBageType> bagType = new List<eBageType>();
									StringBuilder annexRemark = new StringBuilder();
									annexRemark.Append(LanguageMgr.GetTranslation("UserSendMailHandler.AnnexRemark", new object[0]));
									int index = 0;
									if (place != -1)
									{
										ItemInfo goods = player.GetItemAt(bag, place);
										if (goods != null && !goods.IsBinds)
										{
											message.Annex1Name = goods.Template.Name;
											message.Annex1 = goods.ItemID.ToString();
											items.Add(goods);
											bagType.Add(bag);
											index++;
											annexRemark.Append(index);
											annexRemark.Append("、");
											annexRemark.Append(message.Annex1Name);
											annexRemark.Append("x");
											annexRemark.Append(goods.Count);
											annexRemark.Append(";");
										}
									}
									if (place2 != -1)
									{
										ItemInfo goods = player.GetItemAt(bag2, place2);
										if (goods != null && !goods.IsBinds && !items.Contains(goods))
										{
											message.Annex2Name = goods.Template.Name;
											message.Annex2 = goods.ItemID.ToString();
											items.Add(goods);
											bagType.Add(bag2);
											index++;
											annexRemark.Append(index);
											annexRemark.Append("、");
											annexRemark.Append(message.Annex2Name);
											annexRemark.Append("x");
											annexRemark.Append(goods.Count);
											annexRemark.Append(";");
										}
									}
									if (place3 != -1)
									{
										ItemInfo goods = player.GetItemAt(bag3, place3);
										if (goods != null && !goods.IsBinds && !items.Contains(goods))
										{
											message.Annex3Name = goods.Template.Name;
											message.Annex3 = goods.ItemID.ToString();
											items.Add(goods);
											bagType.Add(bag3);
											index++;
											annexRemark.Append(index);
											annexRemark.Append("、");
											annexRemark.Append(message.Annex3Name);
											annexRemark.Append("x");
											annexRemark.Append(goods.Count);
											annexRemark.Append(";");
										}
									}
									if (place4 != -1)
									{
										ItemInfo goods = player.GetItemAt(bag4, place4);
										if (goods != null && !goods.IsBinds && !items.Contains(goods))
										{
											message.Annex4Name = goods.Template.Name;
											message.Annex4 = goods.ItemID.ToString();
											items.Add(goods);
											bagType.Add(bag4);
											index++;
											annexRemark.Append(index);
											annexRemark.Append("、");
											annexRemark.Append(message.Annex4Name);
											annexRemark.Append("x");
											annexRemark.Append(goods.Count);
											annexRemark.Append(";");
										}
									}
									if (isPay)
									{
										if (money <= 0 || (string.IsNullOrEmpty(message.Annex1) && string.IsNullOrEmpty(message.Annex2) && string.IsNullOrEmpty(message.Annex3) && string.IsNullOrEmpty(message.Annex4)))
										{
											result = 1;
											return result;
										}
										message.ValidDate = ((validDate == 1) ? 1 : 6);
										message.Type = 101;
										if (money > 0)
										{
											message.Money = money;
											index++;
											annexRemark.Append(index);
											annexRemark.Append("、");
											annexRemark.Append(LanguageMgr.GetTranslation("UserSendMailHandler.PayMoney", new object[0]));
											annexRemark.Append(money);
											annexRemark.Append(";");
										}
									}
									else
									{
										message.Type = 1;
										if (player.PlayerCharacter.Money >= money && money > 0)
										{
											message.Money = money;
											player.RemoveMoney(money, LogMoneyType.Mail, LogMoneyType.Mail_Send);
											index++;
											annexRemark.Append(index);
											annexRemark.Append("、");
											annexRemark.Append(LanguageMgr.GetTranslation("UserSendMailHandler.Money", new object[0]));
											annexRemark.Append(money);
											annexRemark.Append(";");
										}
									}
									if (annexRemark.Length > 1)
									{
										message.AnnexRemark = annexRemark.ToString();
									}
									if (db.SendMail(message))
									{
										player.RemoveGold(100);
										for (int i = 0; i < items.Count; i++)
										{
											player.TakeOutItem(items[i]);
										}
									}
									player.SaveIntoDatabase();
									pkg.WriteBoolean(true);
									if (user.State != 0)
									{
										player.Out.SendMailResponse(user.ID, eMailRespose.Receiver);
									}
									player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Send);
								}
								else
								{
									msg = "UserSendMailHandler.Failed1";
									pkg.WriteBoolean(false);
								}
							}
							else
							{
								eMsg = eMessageType.ERROR;
								msg = "UserSendMailHandler.Failed2";
								pkg.WriteBoolean(false);
							}
						}
						else
						{
							eMsg = eMessageType.ERROR;
							msg = "UserSendMailHandler.Failed3";
							pkg.WriteBoolean(false);
						}
					}
					player.Out.SendMessage(eMsg, LanguageMgr.GetTranslation(msg, new object[0]));
					player.Out.SendTCP(pkg);
					result = 0;
				}
			}
			return result;
		}
	}
}
