using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
namespace Game.Logic.Spells.FightingSpell
{
	[SpellAttibute(9)]
	public class NoHoleSpell : ISpellHandler
	{
		public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
		{
			new NoHoleEffect(item.Property3).Start(player);
		}
		public void Execute(BaseGame game, Player player, ItemInfo item)
		{
		}
	}
}
