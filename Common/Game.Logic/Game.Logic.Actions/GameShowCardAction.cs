using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;
namespace Game.Logic.Actions
{
	public class GameShowCardAction : BaseAction
	{
		private PVEGame m_game;
		public GameShowCardAction(PVEGame game, int delay, int finishTime) : base(delay, finishTime)
		{
			this.m_game = game;
		}
		protected override void ExecuteImp(BaseGame game, long tick)
		{
			List<Player> players = this.m_game.GetAllFightPlayers();
			foreach (Player p in players)
			{
				if (p.IsActive && p.CanTakeOut > 0)
				{
					p.HasPaymentTakeCard = true;
					int left = p.CanTakeOut;
					for (int i = 0; i < left; i++)
					{
						this.m_game.TakeCard(p, true);
					}
				}
			}
			this.m_game.SendShowCards();
			base.ExecuteImp(game, tick);
		}
	}
}
