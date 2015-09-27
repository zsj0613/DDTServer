using Bussiness;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Quests
{
	public class OwnConsortiaCondition : BaseCondition
	{
		public OwnConsortiaCondition(BaseQuest quest, QuestConditionInfo info, int value) : base(quest, info, value)
		{
		}
		public override void AddTrigger(GamePlayer player)
		{
			player.GuildChanged += new GamePlayer.PlayerOwnConsortiaEventHandle(this.player_OwnConsortia);
		}
		private void player_OwnConsortia()
		{
		}
		public override void RemoveTrigger(GamePlayer player)
		{
			player.GuildChanged -= new GamePlayer.PlayerOwnConsortiaEventHandle(this.player_OwnConsortia);
		}
		public override bool IsCompleted(GamePlayer player)
		{
			bool result = false;
			int tempComp = 0;
			bool result2;
			using (ConsortiaBussiness db = new ConsortiaBussiness())
			{
				ConsortiaInfo info = db.GetConsortiaSingle(player.PlayerCharacter.ConsortiaID);
				switch (this.m_info.Para1)
				{
				case 0:
					tempComp = info.Count;
					break;
				case 1:
					tempComp = player.PlayerCharacter.RichesOffer + player.PlayerCharacter.RichesRob;
					break;
				case 2:
					tempComp = info.SmithLevel;
					break;
				case 3:
					tempComp = info.ShopLevel;
					break;
				case 4:
					tempComp = info.StoreLevel;
					break;
				}
				if (tempComp >= this.m_info.Para2)
				{
					base.Value = 0;
					result = true;
				}
				result2 = result;
			}
			return result2;
		}
	}
}
