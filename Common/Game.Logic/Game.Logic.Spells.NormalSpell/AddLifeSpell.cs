using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
namespace Game.Logic.Spells.NormalSpell
{
	[SpellAttibute(1)]
	public class AddLifeSpell : ISpellHandler
	{
		public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
		{
			switch (item.Property2)
			{
			case 0:
				if (player.IsLiving)
				{
					player.AddBlood(item.Property3);
				}
				break;
			case 1:
			{
				List<Player> temps = player.Game.GetAllFightPlayers();
				foreach (Player p in temps)
				{
					if (p.IsLiving && p.Team == player.Team)
					{
						p.AddBlood(item.Property3);
					}
				}
				break;
			}
			}
		}
		public void Execute(BaseGame game, Player player, ItemInfo item)
		{
		}
	}
}
