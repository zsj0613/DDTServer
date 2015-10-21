using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Game.Server.Achievement
{
	public class AchievementInventory
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private object m_lock;
		protected List<AchievementDataInfo> m_data;
		protected List<UsersRecordInfo> m_userRecord;
		private GamePlayer m_player;
		public AchievementInventory(GamePlayer player)
		{
			this.m_player = player;
			this.m_lock = new object();
			this.m_userRecord = new List<UsersRecordInfo>();
			this.m_data = new List<AchievementDataInfo>();
		}
		public void LoadFromDatabase(int playerId)
		{
			object @lock;
			Monitor.Enter(@lock = this.m_lock);
			try
			{
				using (PlayerBussiness db = new PlayerBussiness())
				{
					this.m_userRecord = db.GetUserRecord(this.m_player.PlayerId);
					this.m_data = db.GetUserAchievementData(this.m_player.PlayerId);
					this.InitUserRecord();
					if (this.m_userRecord != null && this.m_userRecord.Count > 0)
					{
						GSPacketIn pkg = this.m_player.Out.SendInitAchievements(this.m_userRecord);
					}
					if (this.m_data != null && this.m_data.Count > 0)
					{
						GSPacketIn pkg = this.m_player.Out.SendUpdateAchievementData(this.m_data);
					}
				}
				BaseUserRecord.CreateCondition(AchievementMgr.ItemRecordType, this.m_player);
			}
			finally
			{
				Monitor.Exit(@lock);
			}
		}
		public void InitUserRecord()
		{
			Hashtable ht = AchievementMgr.ItemRecordType;
			List<UsersRecordInfo> userRecord;
			Monitor.Enter(userRecord = this.m_userRecord);
			try
			{
				if (this.m_userRecord.Count < ht.Count)
				{
					IDictionaryEnumerator enumerator = ht.GetEnumerator();
					try
					{
						DictionaryEntry de;
						while (enumerator.MoveNext())
						{
							de = (DictionaryEntry)enumerator.Current;
							UsersRecordInfo temp = new UsersRecordInfo();
							temp.UserID = this.m_player.PlayerId;
							UsersRecordInfo arg_95_0 = temp;
							DictionaryEntry de3 = de;
							arg_95_0.RecordID = int.Parse(de3.Key.ToString());
							temp.Total = 0;
							temp.IsDirty = true;
							IEnumerable<UsersRecordInfo> info = this.m_userRecord.Where(delegate(UsersRecordInfo s)
							{
								int arg_1E_0 = s.RecordID;
								DictionaryEntry de2 = de;
								return arg_1E_0 == int.Parse(de2.Key.ToString());
							});
							if (info.ToList<UsersRecordInfo>().Count <= 0)
							{
								this.m_userRecord.Add(temp);
							}
						}
					}
					finally
					{
						IDisposable disposable = enumerator as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
				}
			}
			finally
			{
				Monitor.Exit(userRecord);
			}
		}
		public int UpdateUserAchievement(int type, int value)
		{
			List<UsersRecordInfo> userRecord;
			Monitor.Enter(userRecord = this.m_userRecord);
			try
			{
				foreach (UsersRecordInfo info in this.m_userRecord)
				{
					if (info.RecordID == type)
					{
						info.Total += value;
						info.IsDirty = true;
						GSPacketIn pkg = this.m_player.Out.SendUpdateAchievements(info);
					}
				}
			}
			finally
			{
				Monitor.Exit(userRecord);
			}
			return 0;
		}
		public int UpdateUserAchievement(int type, int value, int mode)
		{
			List<UsersRecordInfo> userRecord;
			Monitor.Enter(userRecord = this.m_userRecord);
			try
			{
				foreach (UsersRecordInfo info in this.m_userRecord)
				{
					if (info.RecordID == type)
					{
						if (info.Total < value)
						{
							info.Total = value;
							info.IsDirty = true;
							GSPacketIn pkg = this.m_player.Out.SendUpdateAchievements(info);
						}
					}
				}
			}
			finally
			{
				Monitor.Exit(userRecord);
			}
			return 0;
		}
		public bool Finish(AchievementInfo achievementInfo)
		{
			bool result;
			if (!this.CanCompleted(achievementInfo) || !this.CheckAchievementData(achievementInfo))
			{
				result = false;
			}
			else
			{
				this.AddAchievementData(achievementInfo);
				this.SendReward(achievementInfo);
				result = true;
			}
			return result;
		}
		private bool CheckAchievementData(AchievementInfo info)
		{
			bool result;
			if (info.EndDate < DateTime.Now)
			{
				result = false;
			}
			else
			{
				if (info.NeedMaxLevel < this.m_player.Level)
				{
					result = false;
				}
				else
				{
					if (info.IsOther == 1 && this.m_player.PlayerCharacter.ConsortiaID <= 0)
					{
						result = false;
					}
					else
					{
						if (info.IsOther == 2 && this.m_player.PlayerCharacter.SpouseID <= 0)
						{
							result = false;
						}
						else
						{
							if (info.PreAchievementID != "0,")
							{
								string[] tempArry = info.PreAchievementID.Split(new char[]
								{
									','
								});
								for (int i = 0; i < tempArry.Length; i++)
								{
									if (!this.IsAchievementFinish(AchievementMgr.GetSingleAchievement(Convert.ToInt32(tempArry[i]))))
									{
										result = false;
										return result;
									}
								}
								result = true;
							}
							else
							{
								result = true;
							}
						}
					}
				}
			}
			return result;
		}
		public bool CanCompleted(AchievementInfo achievementInfo)
		{
			int count = 0;
			List<AchievementConditionInfo> conditions = AchievementMgr.GetAchievementCondition(achievementInfo);
			if (conditions != null && conditions.Count > 0)
			{
				foreach (AchievementConditionInfo condition in conditions)
				{
					foreach (UsersRecordInfo userRecord in this.m_userRecord)
					{
						if (condition.CondictionType == userRecord.RecordID)
						{
							if (condition.Condiction_Para2 <= userRecord.Total)
							{
								count++;
							}
						}
					}
				}
			}
			return count == conditions.Count;
		}
		public bool SendReward(AchievementInfo achievementInfo)
		{
			string msg = "";
			List<AchievementRewardInfo> rewards = AchievementMgr.GetAchievementReward(achievementInfo);
			List<ItemInfo> mainBg = new List<ItemInfo>();
			List<ItemInfo> propBg = new List<ItemInfo>();
			bool result;
			foreach (AchievementRewardInfo reward in rewards)
			{
				if (reward.RewardType == 3)
				{
					ItemTemplateInfo temp = ItemMgr.FindItemTemplate(reward.RewardValueId);
					if (temp != null)
					{
						int NeedSex = this.m_player.PlayerCharacter.Sex ? 1 : 2;
						if (temp.NeedSex != 0 && temp.NeedSex != NeedSex)
						{
							continue;
						}
						int tempCount = reward.RewardCount;
						msg = msg + LanguageMgr.GetTranslation("Game.Server.Achievement.FinishAchievement.RewardProp", new object[]
						{
							temp.Name,
							reward.RewardCount
						}) + " ";
						for (int len = 0; len < tempCount; len += temp.MaxCount)
						{
							int count = (len + temp.MaxCount > tempCount) ? (tempCount - len) : temp.MaxCount;
							ItemInfo item = ItemInfo.CreateFromTemplate(temp, count, 120);
							if (item != null)
							{
								string[] para = reward.RewardPara.Split(new char[]
								{
									','
								});
								item.StrengthenLevel = int.Parse(para[0]);
								item.AttackCompose = int.Parse(para[1]);
								item.DefendCompose = int.Parse(para[2]);
								item.AgilityCompose = int.Parse(para[3]);
								item.LuckCompose = int.Parse(para[4]);
								item.ValidDate = int.Parse(para[5]);
								item.IsBinds = (int.Parse(para[6]) != 0);
								if (temp.BagType == eBageType.PropBag)
								{
									propBg.Add(item);
								}
								else
								{
									mainBg.Add(item);
								}
							}
						}
					}
					if (mainBg.Count > 0 && this.m_player.MainBag.GetEmptyCount() < mainBg.Count)
					{
						this.m_player.Out.SendMessage(eMessageType.ERROR, this.m_player.GetInventoryName(eBageType.MainBag) + LanguageMgr.GetTranslation("Game.Server.Achievement.BagFull", new object[0]) + " ");
						result = false;
						return result;
					}
					if (propBg.Count > 0 && this.m_player.PropBag.GetEmptyCount() < propBg.Count)
					{
						this.m_player.Out.SendMessage(eMessageType.ERROR, this.m_player.GetInventoryName(eBageType.PropBag) + LanguageMgr.GetTranslation("Game.Server.Achievement.BagFull", new object[0]) + " ");
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
				}
			}
			if (achievementInfo.AchievementPoint != 0)
			{
				this.m_player.AddAchievementPoint(achievementInfo.AchievementPoint);
				msg = msg + LanguageMgr.GetTranslation("Game.Server.Achievement.FinishAchievement.AchievementPoint", new object[]
				{
					achievementInfo.AchievementPoint
				}) + " ";
			}
			result = true;
			return result;
		}
		public AchievementInfo FindAchievement(int id)
		{
			AchievementInfo result;
			foreach (KeyValuePair<int, AchievementInfo> info in AchievementMgr.Achievement)
			{
				if (info.Value.ID == id)
				{
					result = info.Value;
					return result;
				}
			}
			result = null;
			return result;
		}
		public bool AddAchievementData(AchievementInfo achievementInfo)
		{
			bool result;
			if (!this.IsAchievementFinish(achievementInfo))
			{
				AchievementDataInfo achievementData = new AchievementDataInfo();
				achievementData.UserID = this.m_player.PlayerId;
				achievementData.AchievementID = achievementInfo.ID;
				achievementData.IsComplete = true;
				achievementData.CompletedDate = DateTime.Now;
				achievementData.IsDirty = true;
				List<AchievementDataInfo> data;
				Monitor.Enter(data = this.m_data);
				try
				{
					this.m_data.Add(achievementData);
				}
				finally
				{
					Monitor.Exit(data);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		private bool IsAchievementFinish(AchievementInfo achievementInfo)
		{
			IEnumerable<AchievementDataInfo> data = 
				from s in this.m_data
				where s.AchievementID == achievementInfo.ID
				select s;
			return data != null && data.ToList<AchievementDataInfo>().Count > 0;
		}
		public void SaveToDatabase()
		{
			if (this.m_userRecord != null && this.m_userRecord.Count > 0)
			{
				List<UsersRecordInfo> userRecord2;
				Monitor.Enter(userRecord2 = this.m_userRecord);
				try
				{
					using (PlayerBussiness db = new PlayerBussiness())
					{
						foreach (UsersRecordInfo userRecord in this.m_userRecord)
						{
							if (userRecord.IsDirty)
							{
								db.UpdateDbUserRecord(userRecord);
							}
						}
					}
				}
				finally
				{
					Monitor.Exit(userRecord2);
				}
			}
			if (this.m_data != null && this.m_data.Count > 0)
			{
				List<AchievementDataInfo> data;
				Monitor.Enter(data = this.m_data);
				try
				{
					using (PlayerBussiness db = new PlayerBussiness())
					{
						foreach (AchievementDataInfo achievementData in this.m_data)
						{
							if (achievementData.IsDirty)
							{
								db.UpdateDbAchievementDataInfo(achievementData);
							}
						}
					}
				}
				finally
				{
					Monitor.Exit(data);
				}
			}
		}
	}
}
