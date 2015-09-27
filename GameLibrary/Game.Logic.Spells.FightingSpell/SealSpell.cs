using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
namespace Game.Logic.Spells.FightingSpell
{
	[SpellAttibute(30)]
	internal class SealSpell : ISpellHandler
	{
		public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
		{
			if (player.IsLiving)
			{
				new SealEffect(item.Property3, 1).Start(player);
			}
		}
		public void Execute(BaseGame game, Player player, ItemInfo item)
		{
		}
	}
}
