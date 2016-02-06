using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class OwnPropertyCondition : BaseCondition
	{
		public OwnPropertyCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
		}
		private void player_OwnProperty()
		{
		}
		public override void RemoveTrigger(GamePlayer player)
		{
		}
		public override bool IsCompleted(GamePlayer player)
		{
			bool result;
			if (player.GetItemCount(this.m_info.Para1) >= this.m_info.Para2)
			{
				base.Value = 0;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
	}
}
