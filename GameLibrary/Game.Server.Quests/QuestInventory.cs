using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.LogEnum;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Game.Server.Quests
{
	public class QuestInventory
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private static ThreadSafeRandom random = new ThreadSafeRandom();
		private object m_lock;
		protected List<BaseQuest> m_list;
		protected ArrayList m_clearList;
		private GamePlayer m_player;
		private byte[] m_states;
		private UnicodeEncoding m_converter;
		protected List<BaseQuest> m_changedQuests = new List<BaseQuest>();
		private int m_changeCount;
		public QuestInventory(GamePlayer player)
		{
			this.m_converter = new UnicodeEncoding();
			this.m_player = player;
			this.m_lock = new object();
			this.m_list = new List<BaseQuest>();
			this.m_clearList = new ArrayList();
		}
		public void LoadFromDatabase(int playerId)
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				this.m_states = ((this.m_player.PlayerCharacter.QuestSite.Count<byte>() == 0) ? this.InitQuest() : this.m_player.PlayerCharacter.QuestSite);
				using (PlayerBussiness db = new PlayerBussiness())
				{
					QuestDataInfo[] datas = db.GetUserQuest(playerId);
					this.BeginChanges();
					QuestDataInfo[] array = datas;
					for (int i = 0; i < array.Length; i++)
					{
						QuestDataInfo dt = array[i];
						QuestInfo info = QuestMgr.GetSingleQuest(dt.QuestID);
						if (info != null && this.CheckQuest(info))
						{
							this.AddQuest(new BaseQuest(info, dt));
						}
					}
					this.CommitChanges();
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
		}
		public void SaveToDatabase()
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				using (PlayerBussiness pb = new PlayerBussiness())
				{
					BaseQuest[] list = new BaseQuest[this.m_list.Count];
					this.m_list.CopyTo(list);
					BaseQuest[] array = list;
					for (int i = 0; i < array.Length; i++)
					{
						BaseQuest q = array[i];
						q.SaveData();
						if (q.Data.IsDirty)
						{
							pb.UpdateDbQuestDataInfo(q.Data);
						}
					}
					list = new BaseQuest[this.m_clearList.Count];
					this.m_clearList.CopyTo(list);
					array = list;
					for (int i = 0; i < array.Length; i++)
					{
						BaseQuest q = array[i];
						q.SaveData();
						pb.UpdateDbQuestDataInfo(q.Data);
					}
					this.m_clearList.Clear();
				}
			}
			finally
			{
				Monitor.Exit(@lock);
			}
		}
		private bool AddQuest(BaseQuest quest)
		{
			List<BaseQuest> list;
			Monitor.Enter(list = this.m_list);
			try
			{
				if (!this.m_list.Contains(quest))
				{
					this.m_list.Add(quest);
				}
			}
			finally
			{
				Monitor.Exit(list);
			}
			quest.Data.IsExist = true;
			quest.AddToPlayer(this.m_player);
			this.OnQuestsChanged(quest);
			return true;
		}
		private bool CheckQuest(QuestInfo info)
		{
			return info != null && !(info.EndDate < DateTime.Now) && info.NeedMaxLevel >= this.m_player.Level && info.NeedMinLevel <= this.m_player.Level;
		}
		public bool AddQuest(QuestInfo info, out string msg)
		{
			msg = "";
			if (info == null)
			{
				msg = "Game.Server.Quests.NoQuest";
			}
			if (info.TimeMode && DateTime.Now.CompareTo(info.StartDate) < 0)
			{
				msg = "Game.Server.Quests.NoTime";
			}
			if (info.TimeMode && DateTime.Now.CompareTo(info.EndDate) > 0)
			{
				msg = "Game.Server.Quests.TimeOver";
			}
			if (this.m_player.PlayerCharacter.Grade < info.NeedMinLevel)
			{
				msg = "Game.Server.Quests.LevelLow";
			}
			if (this.m_player.PlayerCharacter.Grade > info.NeedMaxLevel)
			{
				msg = "Game.Server.Quests.LevelTop";
			}
			if (info.PreQuestID != "0,")
			{
				string[] tempArry = info.PreQuestID.Split(new char[]
				{
					','
				});
				for (int i = 0; i < tempArry.Length - 1; i++)
				{
					if (!this.IsQuestFinish(Convert.ToInt32(tempArry[i])))
					{
						msg = "Game.Server.Quests.NoFinish";
					}
				}
			}
			if (info.IsOther == 1 && this.m_player.PlayerCharacter.ConsortiaID == 0)
			{
				msg = "Game.Server.Quests.NoConsortia";
			}
			if (info.IsOther == 2 && !this.m_player.PlayerCharacter.IsMarried)
			{
				msg = "Game.Server.Quest.QuestInventory.HaveMarry";
			}
			BaseQuest oldData = this.FindQuest(info.ID);
			if (oldData != null && !oldData.Data.IsComplete)
			{
				msg = "Game.Server.Quests.Have";
			}
			if (this.IsQuestFinish(info.ID) && !info.CanRepeat)
			{
				msg = "Game.Server.Quests.NoRepeat";
			}
			if (oldData != null && TimeHelper.GetDaysBetween(oldData.Data.CompletedDate, DateTime.Now) < info.RepeatInterval && oldData.Data.RepeatFinish < 1)
			{
				msg = "Game.Server.Quests.Reset";
			}
			bool result;
			if (msg == "")
			{
				List<QuestConditionInfo> info_condition = QuestMgr.GetQuestCondiction(info);
				int rand = 1;
				if (QuestInventory.random.Next(1000000) <= info.Rands)
				{
					rand = info.RandDouble;
				}
				this.BeginChanges();
				if (oldData == null)
				{
					oldData = new BaseQuest(info, new QuestDataInfo());
				}
				oldData.Reset(this.m_player, rand);
				this.AddQuest(oldData);
				this.CommitChanges();
				result = true;
			}
			else
			{
				msg = LanguageMgr.GetTranslation(msg, new object[0]);
				result = false;
			}
			return result;
		}
		public bool RemoveQuest(BaseQuest quest)
		{
			bool result = false;
			List<BaseQuest> list;
			Monitor.Enter(list = this.m_list);
			try
			{
				if (this.m_list.Remove(quest))
				{
					if (quest.Info.CanRepeat)
					{
						this.m_list.Add(quest);
					}
					else
					{
						this.m_clearList.Add(quest);
						quest.Data.IsExist = false;
					}
					result = true;
				}
			}
			finally
			{
				Monitor.Exit(list);
			}
			if (result)
			{
				quest.RemoveFromPlayer(this.m_player);
				this.OnQuestsChanged(quest);
			}
			return result;
		}
		public void Update(BaseQuest quest)
		{
			if (quest != null)
			{
				this.OnQuestsChanged(quest);
			}
		}
		public bool Finish(BaseQuest baseQuest, int selectedItem)
		{
			bool result;
			if (!baseQuest.CanCompleted(this.m_player))
			{
				result = false;
			}
			else
			{
				string msg = "";
				string RewardBuffName = string.Empty;
				QuestInfo qinfo = baseQuest.Info;
				QuestDataInfo qdata = baseQuest.Data;
				this.m_player.BeginAllChanges();
				try
				{
					if (baseQuest.Finish(this.m_player))
					{
						List<QuestAwardInfo> awards = QuestMgr.GetQuestGoods(qinfo);
						List<ItemInfo> mainBg = new List<ItemInfo>();
						List<ItemInfo> propBg = new List<ItemInfo>();
						foreach (QuestAwardInfo award in awards)
						{
							if (!award.IsSelect || award.RewardItemID == selectedItem)
							{
								ItemTemplateInfo temp = ItemMgr.FindItemTemplate(award.RewardItemID);
								if (temp != null)
								{
									int NeedSex = this.m_player.PlayerCharacter.Sex ? 1 : 2;
									if (temp.NeedSex == 0 || temp.NeedSex == NeedSex)
									{
										int tempCount = award.RewardItemCount;
										if (award.IsCount)
										{
											tempCount *= qdata.RandDobule;
											msg = msg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardProp", new object[]
											{
												temp.Name,
												tempCount
											}) + " ";
										}
										else
										{
											msg = msg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardProp", new object[]
											{
												temp.Name,
												award.RewardItemCount
											}) + " ";
										}
										for (int len = 0; len < tempCount; len += temp.MaxCount)
										{
											int count = (len + temp.MaxCount > tempCount) ? (tempCount - len) : temp.MaxCount;
											ItemInfo item = ItemInfo.CreateFromTemplate(temp, count, 106);
											if (item != null)
											{
												item.ValidDate = award.RewardItemValid;
												item.IsBinds = true;
												item.StrengthenLevel = award.StrengthenLevel;
												item.AttackCompose = award.AttackCompose;
												item.DefendCompose = award.DefendCompose;
												item.AgilityCompose = award.AgilityCompose;
												item.LuckCompose = award.LuckCompose;
												if (temp.BagType == eBageType.PropBag)
												{
													propBg.Add(item);
												}
												else
												{
													mainBg.Add(item);
												}
												if (temp.TemplateID == 11408)
												{
													this.m_player.OnPlayerAddItem("Medal", count);
												}
											}
										}
									}
								}
							}
						}
						if (mainBg.Count > 0 && this.m_player.MainBag.GetEmptyCount() < mainBg.Count)
						{
							baseQuest.CancelFinish(this.m_player);
							this.m_player.Out.SendMessage(eMessageType.ERROR, this.m_player.GetInventoryName(eBageType.MainBag) + LanguageMgr.GetTranslation("Game.Server.Quests.BagFull", new object[0]) + " ");
							result = false;
							return result;
						}
						if (propBg.Count > 0 && this.m_player.PropBag.GetEmptyCount() < propBg.Count)
						{
							baseQuest.CancelFinish(this.m_player);
							this.m_player.Out.SendMessage(eMessageType.ERROR, this.m_player.GetInventoryName(eBageType.PropBag) + LanguageMgr.GetTranslation("Game.Server.Quests.BagFull", new object[0]) + " ");
							result = false;
							return result;
						}
						foreach (ItemInfo item in mainBg)
						{
							//ItemInfo item;
							if (!this.m_player.MainBag.StackItemToAnother(item))
							{
								this.m_player.MainBag.AddItem(item);
							}
						}
						foreach (ItemInfo item in propBg)
						{
							//ItemInfo item;
							if (!this.m_player.PropBag.StackItemToAnother(item))
							{
								this.m_player.PropBag.AddItem(item);
							}
						}
						msg = LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.Reward", new object[0]) + msg;
						if (qinfo.RewardBuffID > 0 && qinfo.RewardBuffDate > 0)
						{
							ItemTemplateInfo temp = ItemMgr.FindItemTemplate(qinfo.RewardBuffID);
							if (temp != null)
							{
								int RewardBuffTime = qinfo.RewardBuffDate * qdata.RandDobule;
								AbstractBuffer buffer = BufferList.CreateBufferHour(temp, RewardBuffTime);
								buffer.Start(this.m_player);
								msg = msg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardBuff", new object[]
								{
									temp.Name,
									RewardBuffTime
								}) + " ";
							}
						}
						if (qinfo.RewardGold != 0)
						{
							int rewardGold = qinfo.RewardGold * qdata.RandDobule;
							this.m_player.AddGold(rewardGold);
							msg = msg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardGold", new object[]
							{
								rewardGold
							}) + " ";
						}
						if (qinfo.RewardMoney != 0)
						{
							int rewardMoney = qinfo.RewardMoney * qdata.RandDobule;
							this.m_player.AddMoney(qinfo.RewardMoney * qdata.RandDobule, LogMoneyType.Award, LogMoneyType.Award_Quest);
							msg = msg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardMoney", new object[]
							{
								rewardMoney
							}) + " ";
						}
						if (qinfo.RewardGP != 0)
						{
							int rewardGp = qinfo.RewardGP * qdata.RandDobule;
							this.m_player.AddGpDirect(rewardGp);
							msg = msg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardGB1", new object[]
							{
								rewardGp
							}) + " ";
						}
						if (qinfo.RewardRiches != 0 && this.m_player.PlayerCharacter.ConsortiaID != 0)
						{
							int riches = qinfo.RewardRiches * qdata.RandDobule;
							this.m_player.AddRichesOffer(riches);
							using (ConsortiaBussiness db = new ConsortiaBussiness())
							{
								db.ConsortiaRichAdd(this.m_player.PlayerCharacter.ConsortiaID, ref riches);
							}
							msg = msg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardRiches", new object[]
							{
								riches
							}) + " ";
						}
						if (qinfo.RewardOffer != 0)
						{
							int rewardOffer = qinfo.RewardOffer * qdata.RandDobule;
							this.m_player.AddOffer(rewardOffer, false);
							msg = msg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardOffer", new object[]
							{
								rewardOffer
							}) + " ";
						}
						if (qinfo.RewardGiftToken != 0)
						{
							int rewardGiftToken = qinfo.RewardGiftToken * qdata.RandDobule;
							this.m_player.AddGiftToken(rewardGiftToken);
							msg += LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardGiftToken", new object[]
							{
								rewardGiftToken + " "
							});
						}
						this.m_player.Out.SendMessage(eMessageType.Normal, msg);
						this.RemoveQuest(baseQuest);
						this.SetQuestFinish(baseQuest.Info.ID);
						this.m_player.PlayerCharacter.QuestSite = this.m_states;
					}
					this.OnQuestsChanged(baseQuest);
				}
				catch (Exception ex)
				{
					if (QuestInventory.log.IsErrorEnabled)
					{
						QuestInventory.log.Error("Quest Finish：" + ex);
					}
					result = false;
					return result;
				}
				finally
				{
					this.m_player.CommitAllChanges();
				}
				this.m_player.OnPlayerQuestFinish(baseQuest);
				result = true;
			}
			return result;
		}
		public BaseQuest FindQuest(int id)
		{
			BaseQuest result;
			foreach (BaseQuest info in this.m_list)
			{
				if (info.Info.ID == id)
				{
					result = info;
					return result;
				}
			}
			result = null;
			return result;
		}
		protected void OnQuestsChanged(BaseQuest quest)
		{
			if (quest != null)
			{
				if (!this.m_changedQuests.Contains(quest))
				{
					List<BaseQuest> changedQuests;
					Monitor.Enter(changedQuests = this.m_changedQuests);
					try
					{
						this.m_changedQuests.Add(quest);
					}
					finally
					{
						Monitor.Exit(changedQuests);
					}
				}
				if (this.m_changeCount <= 0 && this.m_changedQuests.Count > 0)
				{
					this.UpdateChangedQuests();
				}
			}
		}
		private void BeginChanges()
		{
			Interlocked.Increment(ref this.m_changeCount);
		}
		private void CommitChanges()
		{
			int changes = Interlocked.Decrement(ref this.m_changeCount);
			if (changes < 0)
			{
				if (QuestInventory.log.IsErrorEnabled)
				{
					QuestInventory.log.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
				}
				Thread.VolatileWrite(ref this.m_changeCount, 0);
			}
			if (changes <= 0 && this.m_changedQuests.Count > 0)
			{
				this.UpdateChangedQuests();
			}
		}
		public void UpdateChangedQuests()
		{
			if (this.m_changedQuests.Count > 0)
			{
				BaseQuest[] changedQuests = null;
				List<BaseQuest> changedQuests2;
				Monitor.Enter(changedQuests2 = this.m_changedQuests);
				try
				{
					changedQuests = this.m_changedQuests.ToArray();
					this.m_changedQuests.Clear();
				}
				finally
				{
					Monitor.Exit(changedQuests2);
				}
				if (changedQuests != null && this.m_states != null)
				{
					GSPacketIn pkg = this.m_player.Out.SendUpdateQuests(this.m_player, this.m_states, changedQuests);
				}
			}
		}
		private byte[] InitQuest()
		{
			byte[] tempByte = new byte[200];
			for (int i = 0; i < 200; i++)
			{
				tempByte[i] = 0;
			}
			return tempByte;
		}
		private bool SetQuestFinish(int questId)
		{
			bool result;
			if (questId > this.m_states.Length * 8 || questId < 1)
			{
				result = false;
			}
			else
			{
				questId--;
				int index = questId / 8;
				int offset = questId % 8;
				byte[] expr_39_cp_0 = this.m_states;
				int expr_39_cp_1 = index;
				expr_39_cp_0[expr_39_cp_1] |= (byte)(1 << offset);
				result = true;
			}
			return result;
		}
		private bool IsQuestFinish(int questId)
		{
			bool result2;
			if (questId > this.m_states.Length * 8 || questId < 1)
			{
				result2 = false;
			}
			else
			{
				questId--;
				int index = questId / 8;
				int offset = questId % 8;
				int result = (int)this.m_states[index] & 1 << offset;
				result2 = (result != 0);
			}
			return result2;
		}
		public bool ClearConsortiaQuest()
		{
			return true;
		}
		public bool ClearMarryQuest()
		{
			return true;
		}
	}
}
