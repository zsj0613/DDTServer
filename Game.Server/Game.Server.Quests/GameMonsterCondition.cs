using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class GameMonsterCondition : BaseCondition
	{
		public GameMonsterCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.AfterKillingLiving += new GamePlayer.PlayerGameKillEventHandel(this.player_AfterKillingLiving);
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.AfterKillingLiving -= new GamePlayer.PlayerGameKillEventHandel(this.player_AfterKillingLiving);
		}
		private void player_AfterKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage, bool isSpanArea)
		{
			if (type == 2 && id == this.m_info.Para1 && base.Value > 0 && !isLiving)
			{
				base.Value--;
			}
		}
		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value <= 0;
		}
	}
}
