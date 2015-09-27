using Bussiness.Managers;
using Game.Logic;
using Game.Logic.LogEnum;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Server.Quests
{
	public class TurnPropertyCondition : BaseCondition
	{
		private BaseQuest m_quest;
		private GamePlayer m_player;
		public TurnPropertyCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
			this.m_quest = quest;
		}
		public override void AddTrigger(GamePlayer player)
		{
			this.m_player = player;
			player.GameKillDrop += new GamePlayer.GameKillDropEventHandel(this.PveQuestDropItem);
			player.GameOver += new GamePlayer.PlayerGameOverEventHandle(this.PvpQuestDropItem);
			base.AddTrigger(player);
		}
		public override bool IsCompleted(GamePlayer player)
		{
			bool result = false;
			if (player.GetItemCount(this.m_info.Para1) + player.HideBag.GetItemCount(this.m_info.Para1) >= this.m_info.Para2)
			{
				base.Value = 0;
				result = true;
			}
			return result;
		}
		public override bool Finish(GamePlayer player)
		{
			return player.RemoveTemplate(this.m_info.Para1, this.m_info.Para2, eItemRemoveType.Task);
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.GameKillDrop -= new GamePlayer.GameKillDropEventHandel(this.PveQuestDropItem);
			player.GameOver -= new GamePlayer.PlayerGameOverEventHandle(this.PvpQuestDropItem);
			base.RemoveTrigger(player);
		}
		public override bool CancelFinish(GamePlayer player)
		{
			ItemTemplateInfo template = ItemMgr.FindItemTemplate(this.m_info.Para1);
			bool result;
			if (template != null)
			{
				ItemInfo item = ItemInfo.CreateFromTemplate(template, this.m_info.Para2, 117);
				item.IsBinds = true;
				result = player.AddTemplate(item, template.BagType, this.m_info.Para2);
			}
			else
			{
				result = false;
			}
			return result;
		}
		private void PveQuestDropItem(AbstractGame game, int npcId, bool playResult)
		{
			int haveItemCount = this.m_player.GetItemCount(this.m_info.Para1) + this.m_player.TempBag.GetItemCount(this.m_info.Para1);
			if (haveItemCount < this.m_info.Para2)
			{
				List<ItemInfo> infos = null;
				int golds = 0;
				int moneys = 0;
				int gifttokens = 0;
				switch (game.GameType)
				{
				case eGameType.Exploration:
				case eGameType.Boss:
				case eGameType.Treasure:
					DropInventory.PvEQuestsDrop(npcId, this.m_info.Para1, ref infos);
					break;
				}
				if (infos != null)
				{
					foreach (ItemInfo info in infos)
					{
						ItemInfo tempItem = ItemInfo.FindSpecialItemInfo(info, ref golds, ref moneys, ref gifttokens);
						if (haveItemCount + info.Count > this.m_info.Para2)
						{
							info.Count = this.m_info.Para2 - haveItemCount;
							if (info.Count <= 0)
							{
								continue;
							}
						}
						if (tempItem != null && info.Count > 0)
						{
							this.m_player.TempBag.AddTemplate(tempItem, info.Count);
						}
					}
					this.m_player.AddGold(golds);
					this.m_player.AddGiftToken(gifttokens);
					this.m_player.AddMoney(moneys, LogMoneyType.Award, LogMoneyType.Award_Drop);
				}
			}
		}
		private void PvpQuestDropItem(AbstractGame game, bool isWin, int gainXp, bool isSpanArea, bool isCouple)
		{
			int haveItemCount = this.m_player.GetItemCount(this.m_info.Para1) + this.m_player.TempBag.GetItemCount(this.m_info.Para1);
			if (haveItemCount < this.m_info.Para2)
			{
				List<ItemInfo> infos = null;
				int golds = 0;
				int moneys = 0;
				int gifttokens = 0;
				switch (game.GameType)
				{
				case eGameType.Free:
				case eGameType.Guild:
				case eGameType.Training:
				case eGameType.ALL:
					DropInventory.PvPQuestsDrop(this.m_info.Para1, isWin, ref infos);
					break;
				}
				if (infos != null)
				{
					foreach (ItemInfo info in infos)
					{
						ItemInfo tempItem = ItemInfo.FindSpecialItemInfo(info, ref golds, ref moneys, ref gifttokens);
						if (haveItemCount + info.Count > this.m_info.Para2)
						{
							info.Count = this.m_info.Para2 - haveItemCount;
							if (info.Count <= 0)
							{
								continue;
							}
						}
						if (tempItem != null && info.Count > 0)
						{
							this.m_player.TempBag.AddTemplate(tempItem, info.Count);
						}
					}
					this.m_player.AddGold(golds);
					this.m_player.AddGiftToken(gifttokens);
					this.m_player.AddMoney(moneys, LogMoneyType.Award, LogMoneyType.Award_Drop);
				}
			}
		}
	}
}
