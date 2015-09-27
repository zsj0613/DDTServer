using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class UsingItemCondition : BaseCondition
	{
		public UsingItemCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.AfterUsingItem += new GamePlayer.PlayerItemPropertyEventHandle(this.player_ItemProperty);
		}
		private void player_ItemProperty(int templateID)
		{
			if (templateID == this.m_info.Para1 && base.Value > 0)
			{
				base.Value--;
			}
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.AfterUsingItem -= new GamePlayer.PlayerItemPropertyEventHandle(this.player_ItemProperty);
		}
		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value <= 0;
		}
	}
}
