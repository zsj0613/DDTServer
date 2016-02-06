using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
namespace Game.Logic.Spells.FightingSpell
{
	[SpellAttibute(8)]
	public class BreachDefenceSpell : ISpellHandler
	{
		public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
		{
			player.IgnoreArmor = true;
		}
		public void Execute(BaseGame game, Player player, ItemInfo item)
		{
		}
	}
}
