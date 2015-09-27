using Bussiness.Managers;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Quests
{
	public class BaseQuest
	{
		private QuestInfo m_info;
		private QuestDataInfo m_data;
		private List<BaseCondition> m_list;
		private GamePlayer m_player;
		private DateTime m_oldFinishDate;
		public QuestInfo Info
		{
			get
			{
				return this.m_info;
			}
		}
		public QuestDataInfo Data
		{
			get
			{
				return this.m_data;
			}
		}
		public BaseQuest(QuestInfo info, QuestDataInfo data)
		{
			this.m_info = info;
			this.m_data = data;
			this.m_data.QuestID = this.m_info.ID;
			this.m_list = new List<BaseCondition>();
			List<QuestConditionInfo> list = QuestMgr.GetQuestCondiction(info);
			int index = 0;
			foreach (QuestConditionInfo ci in list)
			{
				BaseCondition cd = BaseCondition.CreateCondition(this, ci, data.GetConditionValue(index++));
				if (cd != null)
				{
					this.m_list.Add(cd);
				}
			}
		}
		public BaseCondition GetConditionById(int id)
		{
			BaseCondition result;
			foreach (BaseCondition cd in this.m_list)
			{
				if (cd.Info.CondictionID == id)
				{
					result = cd;
					return result;
				}
			}
			result = null;
			return result;
		}
		public void AddToPlayer(GamePlayer player)
		{
			this.m_player = player;
			if (!this.m_data.IsComplete)
			{
				this.AddTrigger(player);
			}
		}
		public void RemoveFromPlayer(GamePlayer player)
		{
			if (!this.m_data.IsComplete)
			{
				this.RemveTrigger(player);
				this.m_data.RepeatFinish++;
				if (this.m_info.CanRepeat)
				{
					this.m_data.IsComplete = true;
				}
			}
			this.m_player = null;
		}
		public void Reset(GamePlayer player, int rand)
		{
			this.m_data.QuestID = this.m_info.ID;
			this.m_data.UserID = player.PlayerId;
			this.m_data.IsComplete = false;
			if (this.m_data.CompletedDate == DateTime.MinValue)
			{
				this.m_data.CompletedDate = new DateTime(2000, 1, 1);
			}
			if (TimeHelper.GetDaysBetween(this.m_data.CompletedDate, DateTime.Now) >= this.m_info.RepeatInterval || this.m_data.RepeatFinish > this.m_info.RepeatMax)
			{
				this.m_data.RepeatFinish = this.m_info.RepeatMax;
			}
			this.m_data.RepeatFinish--;
			this.m_data.RandDobule = rand;
			foreach (BaseCondition cd in this.m_list)
			{
				cd.Reset(player);
			}
			this.SaveData();
		}
		private void AddTrigger(GamePlayer player)
		{
			foreach (BaseCondition cd in this.m_list)
			{
				cd.AddTrigger(player);
			}
		}
		private void RemveTrigger(GamePlayer player)
		{
			foreach (BaseCondition cd in this.m_list)
			{
				cd.RemoveTrigger(player);
			}
		}
		public void SaveData()
		{
			int index = 0;
			foreach (BaseCondition cd in this.m_list)
			{
				this.m_data.SaveConditionValue(index++, cd.Value);
			}
		}
		public void Update()
		{
			this.SaveData();
			if (this.m_data.IsDirty && this.m_player != null)
			{
				this.m_player.QuestInventory.Update(this);
			}
		}
		public bool CanCompleted(GamePlayer player)
		{
			bool result;
			if (this.m_data.IsComplete)
			{
				result = false;
			}
			else
			{
				foreach (BaseCondition cd in this.m_list)
				{
					if (!cd.IsCompleted(player))
					{
						result = false;
						return result;
					}
				}
				result = true;
			}
			return result;
		}
		public bool Finish(GamePlayer player)
		{
			bool result;
			if (this.CanCompleted(player))
			{
				foreach (BaseCondition cd in this.m_list)
				{
					if (!cd.Finish(player))
					{
						result = false;
						return result;
					}
				}
				this.RemveTrigger(player);
				this.m_data.IsComplete = true;
				this.m_oldFinishDate = this.m_data.CompletedDate;
				this.m_data.CompletedDate = DateTime.Now;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
		public bool CancelFinish(GamePlayer player)
		{
			this.m_data.IsComplete = false;
			this.m_data.CompletedDate = this.m_oldFinishDate;
			foreach (BaseCondition cd in this.m_list)
			{
				cd.CancelFinish(player);
			}
			return true;
		}
	}
}
