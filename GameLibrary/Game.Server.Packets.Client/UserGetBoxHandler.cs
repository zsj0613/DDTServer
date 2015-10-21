using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace Game.Server.Packets.Client
{
	[PacketHandler(53, "获取箱子")]
	public class UserGetBoxHandler : AbstractPlayerPacketHandler
	{
		public override int HandlePacket(GamePlayer player, GSPacketIn packet)
		{
			int operate = packet.ReadInt();
			BoxInfo box = null;
			int result2;
			if (operate == 0)
			{
				int time = packet.ReadInt();
				int onlineTime = (int)DateTime.Now.Subtract(player.BoxBeginTime).TotalMinutes;
				box = BoxMgr.FindTemplateByCondition(0, player.PlayerCharacter.Grade, player.PlayerCharacter.BoxProgression);
				if (box != null && onlineTime >= time && box.Condition == time)
				{
					using (ProduceBussiness db = new ProduceBussiness())
					{
						db.UpdateBoxProgression(player.PlayerCharacter.ID, player.PlayerCharacter.BoxProgression, player.PlayerCharacter.GetBoxLevel, player.PlayerCharacter.AddGPLastDate, DateTime.Now, time);
						player.PlayerCharacter.AlreadyGetBox = time;
						player.PlayerCharacter.BoxGetDate = DateTime.Now;
					}
				}
				result2 = 0;
			}
			else
			{
				int type = packet.ReadInt();
				GSPacketIn pkg = packet.Clone();
				pkg.ClearContext();
				bool updatedb = false;
				bool result = true;
				if (type == 0)
				{
					int onlineTime = (int)DateTime.Now.Subtract(player.BoxBeginTime).TotalMinutes;
					box = BoxMgr.FindTemplateByCondition(0, player.PlayerCharacter.Grade, player.PlayerCharacter.BoxProgression);
					if (box != null && (onlineTime >= box.Condition || player.PlayerCharacter.AlreadyGetBox == box.Condition))
					{
						using (ProduceBussiness db = new ProduceBussiness())
						{
							if (db.UpdateBoxProgression(player.PlayerCharacter.ID, box.Condition, player.PlayerCharacter.GetBoxLevel, player.PlayerCharacter.AddGPLastDate, DateTime.Now.Date, 0))
							{
								player.PlayerCharacter.BoxProgression = box.Condition;
								player.PlayerCharacter.BoxGetDate = DateTime.Now.Date;
								player.PlayerCharacter.AlreadyGetBox = 0;
								updatedb = true;
							}
						}
					}
				}
				else
				{
					box = BoxMgr.FindTemplateByCondition(1, player.PlayerCharacter.GetBoxLevel, Convert.ToInt32(player.PlayerCharacter.Sex));
					if (box != null && player.PlayerCharacter.Grade >= box.Level)
					{
						using (ProduceBussiness db = new ProduceBussiness())
						{
							if (db.UpdateBoxProgression(player.PlayerCharacter.ID, player.PlayerCharacter.BoxProgression, box.Level, player.PlayerCharacter.AddGPLastDate, player.PlayerCharacter.BoxGetDate, 0))
							{
								player.PlayerCharacter.GetBoxLevel = box.Level;
								updatedb = true;
							}
						}
					}
				}
				if (updatedb)
				{
					if (box != null)
					{
						List<ItemInfo> mailList = new List<ItemInfo>();
						List<ItemInfo> items = new List<ItemInfo>();
						int gold = 0;
						int money = 0;
						int giftToken = 0;
						int gp = 0;
						ItemBoxMgr.CreateItemBox(Convert.ToInt32(box.Template), items, ref gold, ref money, ref giftToken, ref gp);
						if (gold > 0)
						{
							player.AddGold(gold);
						}
						if (money > 0)
						{
							player.AddMoney(money, LogMoneyType.Award, LogMoneyType.Award);
						}
						if (giftToken > 0)
						{
							player.AddGiftToken(giftToken);
						}
						if (gp > 0)
						{
							player.AddGP(gp);
						}
						foreach (ItemInfo item in items)
						{
							item.RemoveType = 120;
							if (!player.AddItem(item))
							{
								mailList.Add(item);
							}
						}
						if (type == 0)
						{
							player.BoxBeginTime = DateTime.Now;
							box = BoxMgr.FindTemplateByCondition(0, player.PlayerCharacter.Grade, player.PlayerCharacter.BoxProgression);
							if (box != null)
							{
								player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserGetTimeBoxHandler.success", new object[]
								{
									box.Condition
								}));
							}
							else
							{
								player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserGetTimeBoxHandler.todayOver", new object[0]));
							}
						}
						else
						{
							box = BoxMgr.FindTemplateByCondition(1, player.PlayerCharacter.GetBoxLevel, Convert.ToInt32(player.PlayerCharacter.Sex));
							if (box != null)
							{
								player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserGetTimeBoxHandler.level", new object[]
								{
									box.Level
								}));
							}
							else
							{
								player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserGetTimeBoxHandler.over", new object[0]));
							}
						}
						if (mailList.Count > 0)
						{
							if (player.SendItemsToMail(mailList, LanguageMgr.GetTranslation("UserGetTimeBoxHandler.mail", new object[0]), LanguageMgr.GetTranslation("UserGetTimeBoxHandler.title", new object[0]), eMailType.OpenUpArk))
							{
								player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserGetTimeBixHandler..full", new object[0]));
								result = true;
								player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
							}
						}
					}
					else
					{
						result = false;
					}
				}
				else
				{
					player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserGetTimeBoxHandler.fail", new object[0]));
				}
				if (type == 0)
				{
					pkg.WriteBoolean(result);
					pkg.WriteInt(player.PlayerCharacter.BoxProgression);
					player.SendTCP(pkg);
				}
				result2 = 0;
			}
			return result2;
		}
	}
}
