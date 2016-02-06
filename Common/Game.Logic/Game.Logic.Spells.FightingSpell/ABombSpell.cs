using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
namespace Game.Logic.Spells.FightingSpell
{
	[SpellAttibute(10)]
	public class ABombSpell : ISpellHandler
	{
		public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
		{
			player.SetBall(4);
		}
		public void Execute(BaseGame game, Player player, ItemInfo item)
		{
		}
	}
}
