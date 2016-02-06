using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
namespace Game.Logic.Spells.NormalSpell
{
	[SpellAttibute(5)]
	public class CarrySpell : ISpellHandler
	{
		public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
		{
			player.SetBall(3);
		}
		public void Execute(BaseGame game, Player player, ItemInfo item)
		{
		}
	}
}
