using Game.Server.GameObjects;
using System;
namespace Game.Server.Achievement
{
	public class OwnAddBloodGunCondition : BaseUserRecord
	{
		public OwnAddBloodGunCondition(GamePlayer player, int type) : base(player, type)
		{
			this.AddTrigger(player);
		}
		public override void AddTrigger(GamePlayer player)
		{
			this.m_player.AchievementInventory.UpdateUserAchievement(this.m_type, player.GetItemCount(17002));
		}
	}
}
