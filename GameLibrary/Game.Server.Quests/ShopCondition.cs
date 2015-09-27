using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class ShopCondition : BaseCondition
	{
		public ShopCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.Paid += new GamePlayer.PlayerShopEventHandle(this.player_Shop);
		}
		private void player_Shop(int money, int gold, int offer, int gifttoken, string payGoods)
		{
			if (this.m_info.Para1 == -1 && money > 0)
			{
				base.Value -= money;
			}
			if (this.m_info.Para1 == -2 && gold > 0)
			{
				base.Value -= gold;
			}
			if (this.m_info.Para1 == -3 && offer > 0)
			{
				base.Value -= offer;
			}
			if (this.m_info.Para1 == -4 && gifttoken > 0)
			{
				base.Value -= gifttoken;
			}
			string[] pay = payGoods.Split(new char[]
			{
				','
			});
			string[] array = pay;
			for (int j = 0; j < array.Length; j++)
			{
				string i = array[j];
				if (i == this.m_info.Para1.ToString())
				{
					base.Value--;
				}
			}
			if (base.Value < 0)
			{
				base.Value = 0;
			}
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.Paid -= new GamePlayer.PlayerShopEventHandle(this.player_Shop);
		}
		public override bool IsCompleted(GamePlayer player)
		{
			return base.Value <= 0;
		}
	}
}
