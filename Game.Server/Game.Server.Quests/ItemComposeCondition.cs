using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class ItemComposeCondition : BaseCondition
	{
		public ItemComposeCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.ItemCompose += new GamePlayer.PlayerItemComposeEventHandle(this.player_ItemCompose);
		}
		private void player_ItemCompose(int templateID)
		{
			if (templateID == this.m_info.Para1 && base.Value > 0)
			{
				base.Value--;
			}
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.ItemCompose -= new GamePlayer.PlayerItemComposeEventHandle(this.player_ItemCompose);
		}
		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value <= 0;
		}
	}
}
