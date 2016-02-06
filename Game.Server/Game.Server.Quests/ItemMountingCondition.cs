using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class ItemMountingCondition : BaseCondition
	{
		public ItemMountingCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override bool IsCompleted(GamePlayer player)
		{
			return player.MainBag.GetItemCount(0, this.m_info.Para1) >= this.m_info.Para2;
		}
	}
}
