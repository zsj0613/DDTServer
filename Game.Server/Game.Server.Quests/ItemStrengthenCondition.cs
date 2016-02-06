using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Quests
{
	public class ItemStrengthenCondition : BaseCondition
	{
		public ItemStrengthenCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
		}
		private void player_ItemStrengthen(int categoryID, int level)
		{
		}
		public override void RemoveTrigger(GamePlayer player)
		{
		}
		public override bool IsCompleted(GamePlayer player)
		{
			List<ItemInfo> mainInfos = player.MainBag.GetItems();
			List<ItemInfo> storeInfos = player.StoreBag.GetItems();
			List<ItemInfo> hideInfos = player.HideBag.GetItems();
			foreach (ItemInfo info in mainInfos)
			{
				if (info != null)
				{
					if (this.m_info.Para1 == info.Template.CategoryID && this.m_info.Para2 <= info.StrengthenLevel)
					{
						base.Value = 0;
					}
				}
			}
			foreach (ItemInfo info in storeInfos)
			{
				if (info != null)
				{
					if (this.m_info.Para1 == info.Template.CategoryID && this.m_info.Para2 <= info.StrengthenLevel)
					{
						base.Value = 0;
					}
				}
			}
			foreach (ItemInfo info in hideInfos)
			{
				if (info != null)
				{
					if (this.m_info.Para1 == info.Template.CategoryID && this.m_info.Para2 <= info.StrengthenLevel)
					{
						base.Value = 0;
					}
				}
			}
			return base.Value <= 0;
		}
	}
}
