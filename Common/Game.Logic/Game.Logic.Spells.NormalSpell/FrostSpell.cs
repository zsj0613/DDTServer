using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
namespace Game.Logic.Spells.NormalSpell
{
	[SpellAttibute(2)]
	public class FrostSpell : ISpellHandler
	{
		public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
		{
			player.SetBall(1);
		}
		public void Execute(BaseGame game, Player player, ItemInfo item)
		{
		}
	}
}
