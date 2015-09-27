using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class OwnSpaCondition : BaseCondition
	{
		private GamePlayer m_Player;
		public OwnSpaCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
			this.m_Player = player;
			player.PlayerSpa += new GamePlayer.PlayerOwnSpaEventHandle(this.player_PlayerSpa);
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.PlayerSpa -= new GamePlayer.PlayerOwnSpaEventHandle(this.player_PlayerSpa);
		}
		public void player_PlayerSpa()
		{
			if (base.Value > 0 && this.m_Player.PlayerCharacter.Grade >= 10)
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
