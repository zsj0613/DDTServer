using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
namespace Game.Logic.Spells.FightingSpell
{
	[SpellAttibute(11)]
	public class AttackUpSpell : ISpellHandler
	{
		public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
		{
			player.AddDander(item.Property2);
		}
		public void Execute(BaseGame game, Player player, ItemInfo item)
		{
		}
	}
}
